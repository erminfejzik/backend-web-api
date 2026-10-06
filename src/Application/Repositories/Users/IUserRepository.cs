using Domain.Users;

namespace Application.Repositories.Users
{
    public interface IUserRepository : IGenericRepository
    {
        Task<bool> ExistsByEmailAsync(string email);
        void Add(User user);
    }
}
