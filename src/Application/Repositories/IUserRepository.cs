using Domain.Users;

namespace Application.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<bool> ExistsByEmailAsync(string email);
        void Add(User user);
    }
}
