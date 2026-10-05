using Domain.Common;

namespace Domain.Entities.Users
{
    public sealed class User(Guid id, string email, string password, int roleId) : BaseEntity(id)
    {
        public string Email { get; set; } = email;
        public string Password { get; set; } = password;

        public int RoleId { get; set; } = roleId;
        public Role Role { get; set; } = null!;
    }
}
