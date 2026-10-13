using Domain.Outbox;

namespace Application.Repositories
{
    public interface IOutboxMessageRepository
    {
        ///<summary>
        ///Ids of unfinished messages whose next attempt is due.
        ///</summary>
        Task<IReadOnlyList<Guid>> GetDueIdsAsync(DateTimeOffset nowUtc, int take, CancellationToken ct);

        Task<OutboxMessage?> GetByIdAsync(Guid id, CancellationToken ct);

        void Add(OutboxMessage outboxMessage);
    }
}
