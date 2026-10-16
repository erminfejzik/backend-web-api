using Domain.Users;

namespace Domain.UnitTests.Users
{
    public sealed class UserTests
    {
        const string Email = "user@example.com";
        const string PasswordHash = "hashed-password";
        const RoleId RoleId = RoleId.User;
        static readonly DateTimeOffset Now = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        static readonly TimeSpan Lifetime = TimeSpan.FromHours(24);

        [Fact]
        public void Create_User_SetsEmailPasswordHashAndRoleId()
        {
            var user = User.Create(Email, PasswordHash, RoleId);

            // Assert
            Assert.Equal(Email, user.Email);
            Assert.Equal(PasswordHash, user.PasswordHash);
            Assert.Equal(RoleId, user.RoleId);
        }

        [Fact]
        public void IssueToken_AddsToken_WithRequestedDetails()
        {
            // Arrange
            var user = User.Create(Email, PasswordHash, RoleId);
            const string tokenHash = "verification-token-hash";

            // Act
            var token = user.IssueToken(UserTokenPurpose.VerifyEmail, tokenHash, Lifetime, Now);

            // Assert
            Assert.Same(token, Assert.Single(user.Tokens));
            Assert.Equal(user.Id, token.UserId);
            Assert.Equal(UserTokenPurpose.VerifyEmail, token.Purpose);
            Assert.Equal(Email, token.Email);
            Assert.Equal(tokenHash, token.TokenHash);
        }

        [Fact]
        public void IssueToken_RevokesOldTokens_WithSamePurpose()
        {
            // Arrange
            var user = User.Create(Email, PasswordHash, RoleId);
            const string tokenHash1 = "verification-token-hash-1";
            const string tokenHash2 = "verification-token-hash-2";
            const string tokenHash3 = "verification-token-hash-3";

            // Act
            _ = user.IssueToken(UserTokenPurpose.VerifyEmail, tokenHash1, Lifetime, Now);
            var token2 = user.IssueToken(UserTokenPurpose.ResetPassword, tokenHash2, Lifetime, Now);
            var token3 = user.IssueToken(UserTokenPurpose.VerifyEmail, tokenHash3, Lifetime, Now);

            // Assert
            Assert.Equal(2, user.Tokens.Count);

            Assert.Equal(token2.TokenHash, user.Tokens.First().TokenHash);
            Assert.Equal(token2.Purpose, user.Tokens.First().Purpose);

            Assert.Equal(token3.TokenHash, user.Tokens.Last().TokenHash);
            Assert.Equal(token3.Purpose, user.Tokens.Last().Purpose);
        }

        [Fact]
        public void VerifyEmail_WithValidToken_VerifiesEmailAndConsumesToken()
        {
            // Arrange
            var user = User.Create(Email, PasswordHash, RoleId);
            const string tokenHash = "verification-token-hash";
            var token = user.IssueToken(UserTokenPurpose.VerifyEmail, tokenHash, Lifetime, Now);
            var verifiedAt = Now.AddMinutes(5);

            // Act
            var result = user.VerifyEmail(tokenHash, verifiedAt);

            // Assert
            Assert.True(result);
            Assert.Equal(verifiedAt, user.EmailVerifiedAt);
            Assert.Equal(verifiedAt, token.UsedAt);
        }

        [Fact]
        public void VerifyEmail_WithUnknownTokenHash_ReturnsFalse()
        {
            // Arrange
            var user = User.Create(Email, PasswordHash, RoleId);

            // Act
            var result = user.VerifyEmail("unknown-token-hash", Now);

            // Assert
            Assert.False(result);
            Assert.Null(user.EmailVerifiedAt);
        }

        [Fact]
        public void VerifyEmail_WhenEmailAlreadyVerified_ReturnsFalse()
        {
            // Arrange
            var user = User.Create(Email, PasswordHash, RoleId);
            const string tokenHash = "verification-token-hash";
            user.IssueToken(UserTokenPurpose.VerifyEmail, tokenHash, Lifetime, Now);
            user.VerifyEmail(tokenHash, Now);

            // Act
            var result = user.VerifyEmail(tokenHash, Now.AddMinutes(5));

            // Assert
            Assert.False(result);
        }
    }
}
