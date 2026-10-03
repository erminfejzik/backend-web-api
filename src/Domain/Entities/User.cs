using Domain.Common;

namespace Domain.Entities
{
    public sealed class User(Guid id, string email, string password, DateTimeOffset createdAt) : BaseEntity(id, createdAt)
    {
        public string Email { get; private set; } = email;
        public string Password { get; private set; } = password;
    }
}
