using Application.Abstractions.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;

namespace Infrastructure.Services
{
    internal sealed class EmailService(IOptions<SmtpOptions> options) : IEmailService
    {
        private readonly SmtpOptions _options = options.Value;

        private const string TestInboxEmail = "signup@sandbox.maileroo.io";

        public async Task SendAsync(string receiver, string subject, string htmlBody, CancellationToken ct = default)
        {
            using var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_options.FromName, _options.FromAddress));

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                receiver = TestInboxEmail;
            }
            message.To.Add(MailboxAddress.Parse(receiver));

            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            using var client = new SmtpClient 
            {
                Timeout = _options.TimeoutSeconds * 1000
            };

            try
            {
                await client.ConnectAsync(_options.Host, _options.Port, true, ct);

                if (!string.IsNullOrWhiteSpace(_options.Username))
                    await client.AuthenticateAsync(_options.Username, _options.Password, ct);

                await client.SendAsync(message, ct);
            }
            finally
            {
                if (client.IsConnected)
                    await client.DisconnectAsync(quit: true, CancellationToken.None);
            }
        }
    }

    public sealed class SmtpOptions
    {
        public const string SectionName = "Smtp";

        public string Host { get; set; } = default!;

        public int Port { get; set; } = 587;

        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;

        public string FromAddress { get; set; } = default!;
        public string FromName { get; set; } = "MyApp";

        public int TimeoutSeconds { get; set; } = 15;
    }
}
