using Domain.Common;

namespace Domain.Entities.Users
{
    public sealed class User(Guid Id, DateTimeOffset CreatedAt) : BaseEntity(Id, CreatedAt)
    {
        public required string Email { get; set; }
        public required string Password { get; set; }

        public required int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}
