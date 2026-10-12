using Domain.Users;

namespace Application.Abstractions.Database
{
    /// <summary>
    /// Read-only, non-tracking access to the data for query handlers.
    /// Not for command handlers: they must go through the aggregate repositories
    /// so domain rules are enforced. Always project results into DTOs rather than
    /// returning entities.
    /// </summary>
    public interface IReadDbContext
    {
        IQueryable<User> Users { get; }
    }
}
