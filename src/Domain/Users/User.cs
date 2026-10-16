using Domain.Common;

namespace Domain.Users
{
    public sealed class User : BaseEntity
    {
        public string Email { get; private set; } = default!;
        public string PasswordHash { get; private set; } = default!; // Hashed password

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
            PasswordHash = passwordHash,
            RoleId = roleId
        };

        public UserToken IssueToken(UserTokenPurpose purpose, string tokenHash, TimeSpan lifetime, DateTimeOffset now)
        {
            // Revoke all previous tokens of the same purpose that are still valid
            for (int i = _tokens.Count - 1; i >= 0; i--)
            {
                var token = _tokens[i];
                if (token.Purpose == purpose && token.IsValid(now))
                {
                    token.Revoke(now);
                    _tokens.Remove(token);
                }
            }

            var newToken = UserToken.Create(
                userId: Id,
                purpose: purpose,
                email: Email,
                tokenHash: tokenHash,
                lifetime: lifetime,
                now: now);

            _tokens.Add(newToken);
            return newToken;
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
