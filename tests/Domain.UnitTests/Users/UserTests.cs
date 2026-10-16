using Domain.Users;
using FluentAssertions;

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
        public void Create_User_Should_SetEmailPasswordHashAndRoleId()
        {
            var user = User.Create(Email, PasswordHash, RoleId);

            // Assert
            user.Email.Should().Be(Email);
            user.PasswordHash.Should().Be(PasswordHash);
            user.RoleId.Should().Be(RoleId);
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
            var user = User.Create(Email, PasswordHash, RoleId);
            const string tokenHash1 = "verification-token-hash-1";
            const string tokenHash2 = "verification-token-hash-2";
            const string tokenHash3 = "verification-token-hash-3";

            // Act
            _ = user.IssueToken(UserTokenPurpose.VerifyEmail, tokenHash1, Lifetime, Now);
            var token2 = user.IssueToken(UserTokenPurpose.ResetPassword, tokenHash2, Lifetime, Now);
            var token3 = user.IssueToken(UserTokenPurpose.VerifyEmail, tokenHash3, Lifetime, Now);

            // Assert
            user.Tokens.Should().HaveCount(2);

            user.Tokens.Should().HaveElementAt(0, token2);
            user.Tokens.Should().HaveElementAt(1, token3);
        }

        [Fact]
        public void VerifyEmail_Should_VerifiyEmailAndConsumeToken_WhenTokenIsValid()
        {
            // Arrange
            var user = User.Create(Email, PasswordHash, RoleId);
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
            var user = User.Create(Email, PasswordHash, RoleId);

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
            var user = User.Create(Email, PasswordHash, RoleId);
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
