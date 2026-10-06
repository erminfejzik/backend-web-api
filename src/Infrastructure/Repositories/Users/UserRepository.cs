using Application.Repositories.Users;
using Domain.Users;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Users
{
    internal sealed class UserRepository(ApplicationDbContext dbContext) : GenericRepository(dbContext), IUserRepository
    {
        public Task<bool> ExistsByEmailAsync(string email) => DbContext.Users.AnyAsync(u => u.Email == email);

        public void Add(User user)
        {
            DbContext.Users.Add(user);
        }
    }
}   
