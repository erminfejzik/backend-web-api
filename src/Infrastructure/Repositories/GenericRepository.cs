using Infrastructure.Database;

namespace Infrastructure.Repositories
{
    internal abstract class GenericRepository(ApplicationDbContext context) : Application.Abstractions.IGenericRepository
    {
        protected readonly ApplicationDbContext DbContext = context;

        public Task<int> SaveChangesAsync()
        {
            return DbContext.SaveChangesAsync();
        }
    }
}
