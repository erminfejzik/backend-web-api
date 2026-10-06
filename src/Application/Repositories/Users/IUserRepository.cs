using Domain.Users;

namespace Application.Repositories.Users
{
    public interface IUserRepository : IGenericRepository
    {
        void Add(User user);
    }
}
