using Domain.Users;

namespace Domain.UnitTests.Users
{
    public sealed class UserTests
    {
        const string Email = "user@example.com";
        const string PasswordHash = "hashed-password";
        private const RoleId UserRole = RoleId.User;
        static readonly DateTimeOffset Now = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        static readonly TimeSpan Lifetime = TimeSpan.FromHours(24);

        [Fact]
        public void Create_User_Should_SetEmailPasswordHashAndRoleId()
        {
            var user = User.Create(Email, PasswordHash, UserRole);

            // Assert
            user.Email.Should().Be(Email);
            user.PasswordHash.Should().Be(PasswordHash);
            user.RoleId.Should().Be(UserRole);
        }

        [Fact]
        public void IssueToken_AddsToken_WithRequestedDetails()
        {
            // Arrange
            var user = User.Create(Email, PasswordHash, UserRole);
            const string tokenHash = "verification-token-hash";

            // Act
            var token = user.IssueToken(UserTokenPurpose.VerifyEmail, tokenHash, Lifetime, Now);

            // Assert
            user.Tokens.Should().ContainSingle("because only one token was issued");
            user.Tokens.First().Should().BeSameAs(token);
            token.TokenHash.Should().Be(tokenHash);
            token.Purpose.Should().Be(UserTokenPurpose.VerifyEmail);
            token.Email.Should().Be(Email);
        }

        [Fact]
        public void IssueToken_Should_RevokeOldTokens_WithSamePurpose()
        {
            // Arrange
            var user = User.Create(Email, PasswordHash, UserRole);
            const string tokenHash = "token-hash";

            var firstVerifyEmailToken = user.IssueToken(UserTokenPurpose.VerifyEmail, tokenHash, Lifetime, Now);
            var resetPasswordToken = user.IssueToken(UserTokenPurpose.ResetPassword, tokenHash, Lifetime, Now);

            // Act
            var secondVerifyEmailToken = user.IssueToken(UserTokenPurpose.VerifyEmail, tokenHash, Lifetime, Now);

            // Assert
            user.Tokens.Should().BeEqualTo(firstVerifyEmailToken, resetPasswordToken, secondVerifyEmailToken);
            firstVerifyEmailToken.UsedAt.Should().BeExactly(Now);
            resetPasswordToken.UsedAt.Should().BeNull();
            secondVerifyEmailToken.UsedAt.Should().BeNull();
        }

        [Fact]
        public void IssueToken_ShouldNot_RevokeExpiredTokens_WithSamePurpose()
        {
            // Arrange
            var user = User.Create(Email, PasswordHash, UserRole);
            const string tokenHash = "token-hash";

            var token1 = user.IssueToken(UserTokenPurpose.VerifyEmail, tokenHash, Lifetime, Now);
            user.VerifyEmail(tokenHash, Now);

            var now2 = Now.AddMinutes(5);

            // Act
            _ = user.IssueToken(UserTokenPurpose.VerifyEmail, tokenHash, Lifetime, now2);

            // Assert
            token1.UsedAt.Should().BeExactly(Now); // not now2
        }

        [Fact]
        public void VerifyEmail_Should_VerifyEmailAndConsumeToken_WhenTokenIsValid()
        {
            // Arrange
            var user = User.Create(Email, PasswordHash, UserRole);
            const string tokenHash = "verification-token-hash";
            var token = user.IssueToken(UserTokenPurpose.VerifyEmail, tokenHash, Lifetime, Now);
            var verifiedAt = Now.AddMinutes(5);

            // Act
            var result = user.VerifyEmail(tokenHash, verifiedAt);

            // Assert
            result.Should().BeTrue();
            user.EmailVerifiedAt.Should().BeExactly(verifiedAt);
            token.UsedAt.Should().BeExactly(verifiedAt);
        }

        [Fact]
        public void VerifyEmail_Should_ReturnFalse_WithUnknownTokenHash()
        {
            // Arrange
            var user = User.Create(Email, PasswordHash, UserRole);

            // Act
            var result = user.VerifyEmail("unknown-token-hash", Now);

            // Assert
            result.Should().BeFalse();
            user.EmailVerifiedAt.Should().BeNull();
        }

        [Fact]
        public void VerifyEmail_Should_ReturnFalse_WhenEmailIsAlreadyVerified()
        {
            // Arrange
            var user = User.Create(Email, PasswordHash, UserRole);
            const string tokenHash = "verification-token-hash";
            user.IssueToken(UserTokenPurpose.VerifyEmail, tokenHash, Lifetime, Now);
            user.VerifyEmail(tokenHash, Now);

            // Act
            var result = user.VerifyEmail(tokenHash, Now.AddMinutes(5));

            // Assert
            result.Should().BeFalse();
        }
    }
}
