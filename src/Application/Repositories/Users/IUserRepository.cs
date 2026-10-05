using Application.Abstractions;
using Domain.Entities.Users;

namespace Application.Repositories.Users
{
    public interface IUserRepository : IGenericRepository
    {
        void Add(User user, CancellationToken cancellationToken = default);
    }
}
