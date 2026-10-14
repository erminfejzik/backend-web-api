using Domain.Common;

namespace Domain.Users
{
    public sealed class User : BaseEntity
    {
        public string Email { get; private set; } = default!;
        public string Password { get; private set; } = default!; // Hashed password

        public int RoleId { get; private set; }
        public Role Role { get; private set; } = null!;

        private readonly List<UserToken> _tokens = [];
        public IReadOnlyCollection<UserToken> Tokens => _tokens;

        // --- Account Lifecycle Settings ---
        public bool IsBlocked { get; private set; }
        public DateTimeOffset? EmailVerifiedAt { get; private set; }
        public DateTimeOffset? LastLoginAt { get; private set; }
        public int FailedLoginAttempts { get; private set; }
        public DateTimeOffset? LockoutEnd { get; private set; }

        // --- Security Settings ---
        public bool TwoFactorEnabled { get; private set; }

        public static User Create(string email, string passwordHash, int roleId) =>
        new()
        {
            Email = email,
            Password = passwordHash,
            RoleId = roleId
        };

        public UserToken IssueToken(UserTokenPurpose purpose, string tokenHash, TimeSpan lifetime, DateTimeOffset now)
        {
            foreach (var old in _tokens.Where(t => t.Purpose == purpose && t.UsedAt.HasValue))
                old.Revoke(now);

            var token = UserToken.Create(
                userId: Id,
                purpose: purpose,
                email: Email,
                tokenHash: tokenHash,
                lifetime: lifetime,
                now: now);

            _tokens.Add(token);
            return token;
        }

        public bool VerifyEmail(string tokenHash, DateTimeOffset now)
        {
            if (EmailVerifiedAt.HasValue)
                return false;

            var token = _tokens.SingleOrDefault(t => t.TokenHash == tokenHash);

            if (token is null ||
                !token.IsValid(now) ||
                token.Purpose != UserTokenPurpose.VerifyEmail ||
                token.Email != Email)
                return false;

            token.Revoke(now);

            EmailVerifiedAt = now;

            return true;
        }
    }
}
