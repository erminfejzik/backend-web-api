namespace Application.Abstractions.Database
{
    /// <summary>
    /// Commits all changes made during a single use case as one atomic operation.
    /// Used by command handlers together with the aggregate repositories, which
    /// load and add aggregates but never save on their own.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Persists every tracked change in a single transaction: all of it is
        /// saved or none of it is.
        /// </summary>
        /// <param name="ct">Token used to cancel the operation, typically the request's abort token.</param>
        /// <returns>The number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync(CancellationToken ct);
    }
}
