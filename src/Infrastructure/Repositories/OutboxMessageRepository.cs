using Application.Repositories;
using Domain.Outbox;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    internal sealed class OutboxMessageRepository(ApplicationDbContext db) : IOutboxMessageRepository
    {
        public async Task<IReadOnlyList<Guid>> GetDueIdsAsync(
                    DateTimeOffset nowUtc, int take, CancellationToken ct) =>
                    await db.OutboxMessages
                        .AsNoTracking()
                        .Where(m => m.ProcessedAt == null && m.NextAttemptAt <= nowUtc)
                        .OrderBy(m => m.CreatedAt)
                        .Select(m => m.Id)
                        .Take(take)
                        .ToListAsync(ct);

        public Task<OutboxMessage?> GetByIdAsync(Guid id, CancellationToken ct) =>
            db.OutboxMessages.FirstOrDefaultAsync(m => m.Id == id, ct);

        public void Add(OutboxMessage outboxMessage)
        {
            db.Add(outboxMessage);
        }
    }
}
