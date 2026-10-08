using Domain.Common;

namespace Domain.Users
{
    public sealed class User : BaseEntity
    {
        public required string Email { get; set; }
        public required string Password { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;

        // --- Account Lifecycle Settings ---
        public bool IsBlocked { get; set; }
        public DateTimeOffset? EmailVerifiedAt { get; set; }
        public DateTimeOffset? LastLoginAt { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }

        // --- Security Settings ---
        public bool TwoFactorEnabled { get; set; }
    }
}
