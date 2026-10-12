using Application.Repositories;
using Domain.Users;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    internal sealed class UserRepository(ApplicationDbContext db) : IUserRepository
    {
        public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await db.Users.FindAsync([id], ct);
        }

        public Task<bool> ExistsByEmailAsync(string email) => db.Users.AnyAsync(u => u.Email == email);

        public void Add(User user)
        {
            db.Users.Add(user);
        }
    }
}   
