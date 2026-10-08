namespace Domain.Users
{
    public enum UserTokenPurpose { 
        VerifyEmail = 0,
        ResetPassword = 1
    }

    public class UserToken
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public UserTokenPurpose Purpose { get; set; }
        public string? Email { get; set; } // only for VerifyEmail
        public string TokenHash { get; set; } = null!;
        public DateTimeOffset ExpiresAt { get; set; }
        public DateTimeOffset? UsedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
