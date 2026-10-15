using Domain.Users;

namespace Domain.UnitTests.Users
{
    public sealed class UserTests
    {
        [Fact]
        public void Create_User_SetsEmailPasswordHashAndRoleId()
        {
            // Arrange
            const string email = "user@example.com";
            const string passwordHash = "hashed-password";
            const int roleId = Role.UserId;

            // Act
            var user = User.Create(email, passwordHash, roleId);

            // Assert
            Assert.Equal(email, user.Email);
            Assert.Equal(passwordHash, user.PasswordHash);
            Assert.Equal(roleId, user.RoleId);
        }
    }
}
