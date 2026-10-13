namespace Domain.Users
{
    public enum UserTokenPurpose { 
        VerifyEmail = 0,
        ResetPassword = 1
    }

    public class UserToken
    {
        public long Id { get; init; }
        public Guid UserId { get; private set; }
        public UserTokenPurpose Purpose { get; private set; }
        public string? Email { get; private set; } // only for VerifyEmail
        public string TokenHash { get; private set; } = null!;
        public DateTimeOffset ExpiresAt { get; private set; }
        public DateTimeOffset? UsedAt { get; private set; }
        public DateTimeOffset CreatedAt { get; init; }

        public static UserToken Create(
            Guid userId,
            UserTokenPurpose purpose,
            string email,
            string tokenHash,
            TimeSpan lifetime,
            DateTimeOffset now) => new()
        {
            UserId = userId,
            Purpose = purpose,
            Email = email,
            TokenHash = tokenHash,
            ExpiresAt = now.Add(lifetime),
        };

        public void Revoke(DateTimeOffset now) => UsedAt = now;
    }
}
