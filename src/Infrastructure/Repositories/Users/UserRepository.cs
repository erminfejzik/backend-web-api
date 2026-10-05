using Application.Repositories.Users;
using Domain.Entities.Users;
using Infrastructure.Database;

namespace Infrastructure.Repositories.Users
{
    internal sealed class UserRepository(ApplicationDbContext dbContext) : GenericRepository(dbContext), IUserRepository
    {
        public void Add(User user, CancellationToken cancellationToken = default)
        {
            DbContext.Users.Add(user);
        }
    }
}
