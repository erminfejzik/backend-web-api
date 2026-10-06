using Domain.Common;

namespace Domain.Users
{
    public sealed class User : BaseEntity
    {
        public required string Email { get; set; }
        public required string Password { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}
