using Application.Abstractions.Database;
using Application.Abstractions.Services;
using Application.Repositories;
using Common;
using Domain.Outbox;
using Domain.Users;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.BackgroundServices
{
    public sealed class PermanentOutboxException : Exception
    {
        public PermanentOutboxException(string message) : base(message) { }

        public PermanentOutboxException(string message, Exception innerException) : base(message, innerException) { }

        public PermanentOutboxException() { }
    }

    public sealed partial class OutboxMessageProcessor(
        IServiceScopeFactory scopes,
        ILogger<OutboxMessageProcessor> logger)
        : BackgroundService
    {
        private const int MaxRetries = 2;  // 1 initial attempt + 2 retries = 3 total
        private const int BatchSize = 100;
        private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan BaseDelay = TimeSpan.FromSeconds(30);   // 30s, 60s, 120s

        private readonly IServiceScopeFactory _scopes = scopes;
        private readonly ILogger<OutboxMessageProcessor> _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(PollInterval);

            try
            {
                do
                {
                    try
                    {
                        await ProcessBatchAsync(stoppingToken);
                    }
                    catch (Exception ex) when (ex is not OperationCanceledException)
                    {
                        // e.g. database temporarily unavailable: log and try again on the next tick.
                        // Without this, an unhandled exception would stop the host .
                        LogBatchFailed(_logger, ex);
                    }
                }
                while (await timer.WaitForNextTickAsync(stoppingToken));
            }
            catch (OperationCanceledException)
            {
                // Normal shutdown
            }
        }

        private async Task ProcessBatchAsync(CancellationToken ct)
        {
            IReadOnlyList<Guid> dueIds;

            using (var scope = _scopes.CreateScope())
            {
                var outboxMessageRepository = scope.ServiceProvider.GetRequiredService<IOutboxMessageRepository>();
                var dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

                dueIds = await outboxMessageRepository.GetDueIdsAsync(dateTimeProvider.UtcNow, BatchSize, ct);
            }

            foreach (var id in dueIds)
            {
                ct.ThrowIfCancellationRequested();
                await ProcessMessageAsync(id, ct);
            }
        }

        // One scope (and therefore one DbContext) per message, so a failed SaveChanges
        // or a half-tracked entity can never leak into the next message.
        private async Task ProcessMessageAsync(Guid id, CancellationToken ct)
        {
            try
            {
                using var scope = _scopes.CreateScope();
                var sp = scope.ServiceProvider;

                var uow = sp.GetRequiredService<IUnitOfWork>();
                var outboxMessages = sp.GetRequiredService<IOutboxMessageRepository>();
                var users = sp.GetRequiredService<IUserRepository>();

                var tokenGenerator = sp.GetRequiredService<IUserTokenGenerator>();
                var emailService = sp.GetRequiredService<IEmailService>();
                var dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

                var msg = await outboxMessages.GetByIdAsync(id, ct);
                if (msg is null || msg.ProcessedAt is not null)
                    return;

                var payload = JsonSerializer.Deserialize<VerificationEmailPayload>(msg.Payload)
                    ?? throw new PermanentOutboxException("Outbox payload is empty.");

                var user = await users.GetByIdAsync(payload.UserId, ct)
                    ?? throw new PermanentOutboxException($"User {payload.UserId} no longer exists.");

                if (user.EmailVerifiedAt.HasValue)
                {
                    // Already verified (e.g. via an earlier duplicate email): nothing to send.
                    msg.MarkProcessed(dateTimeProvider.UtcNow);
                    await uow.SaveChangesAsync(ct);
                    return;
                }

                // 1) Persist the token HASH first, so the link in the email always works.
                var (raw, hash) = tokenGenerator.Generate();

                user.IssueToken(
                    purpose: UserTokenPurpose.VerifyEmail,
                    tokenHash: hash,
                    lifetime: TimeSpan.FromHours(24),
                    now: dateTimeProvider.UtcNow);

                await uow.SaveChangesAsync(ct);

                // 2) Send the email containing the RAW token (it exists only in memory + the email).
                var link = $"https://localhost:5001/verify-email?token={Uri.EscapeDataString(raw)}";
                await emailService.SendAsync(user.Email, "Verify your email", BuildBody(link), ct);

                // 3) Mark as processed.
                msg.MarkProcessed(dateTimeProvider.UtcNow);

                using var saveCts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                await uow.SaveChangesAsync(saveCts.Token);

            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw; // shutting down: leave the message untouched, it will be picked up next start
            }
            catch (Exception ex) when (ex is PermanentOutboxException or JsonException)
            {
                await RecordFailureAsync(id, ex, permanent: true, ct);
            }
            catch (Exception ex) when (ex is not PermanentOutboxException) // added  - ex is not PermanentOutboxException to fix warning
            {
                await RecordFailureAsync(id, ex, permanent: false, ct);
            }
        }

        private async Task RecordFailureAsync(Guid id, Exception ex, bool permanent, CancellationToken ct)
        {
            try
            {
                using var scope = _scopes.CreateScope();

                var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var outboxMessagesRepository = scope.ServiceProvider.GetRequiredService<IOutboxMessageRepository>();

                var msg = await outboxMessagesRepository.GetByIdAsync(id, ct);
                if (msg is null) return;

                var error = string.IsNullOrWhiteSpace(ex.Message) ? ex.GetType().Name : ex.Message;
                var gaveUp = msg.RegisterFailure(error, permanent, MaxRetries, BaseDelay, DateTime.UtcNow);

                if (gaveUp)
                {
                    LogMessagePermanentlyFailed(_logger, ex, id, msg.Attempts);
                }

                using var saveCts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                await uow.SaveChangesAsync(saveCts.Token);
            }
            catch (Exception inner) when (inner is not OperationCanceledException)
            {
                // Couldn't even record the failure (DB down). The row stays due and is retried next tick.
                LogCouldNotRecordFailure(_logger, inner, id);
            }
        }

        private static string BuildBody(string link) =>
            $"""
         <p>Welcome! Please confirm your email address.</p>
         <p><a href="{link}">Verify my email</a></p>
         <p>This link expires in 24 hours. If you didn't create an account, ignore this email.</p>
         """;

        [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "Outbox batch failed")]
        private static partial void LogBatchFailed(ILogger logger, Exception exception);

        [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "Outbox message {MessageId} permanently failed after {Attempts} attempt(s)")]
        private static partial void LogMessagePermanentlyFailed(ILogger logger, Exception exception, Guid messageId, int attempts);

        [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "Could not record failure for outbox message {MessageId}")]
        private static partial void LogCouldNotRecordFailure(ILogger logger, Exception exception, Guid messageId);
    }
}
