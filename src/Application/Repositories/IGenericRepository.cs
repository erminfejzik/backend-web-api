namespace Application.Repositories
{
    /// <summary>
    /// Represents a generic repository interface.
    /// Prevent repetitive code for SaveChangesAsync() in each individual repositories.
    /// Does not define other CRUD operations because not all repositories will have the same operations, and some may have additional operations that are not common to all repositories.
    /// </summary>
    public interface IGenericRepository
    {
        Task<int> SaveChangesAsync();
    }
}
