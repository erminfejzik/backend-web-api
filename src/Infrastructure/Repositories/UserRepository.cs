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

        public Task<User?> GetByEmailVerificationTokenHashAsync(string tokenHash, CancellationToken ct) =>
            db.Users
                .Include(user => user.Tokens.Where(token => token.TokenHash == tokenHash))
                .FirstOrDefaultAsync(user => user.Tokens.Any(token => token.TokenHash == tokenHash), ct);

        public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct) => db.Users.AnyAsync(u => u.Email == email, ct);

        public void Add(User user)
        {
            db.Users.Add(user);
        }
    }
}   
