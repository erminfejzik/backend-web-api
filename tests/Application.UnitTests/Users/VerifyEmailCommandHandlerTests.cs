using Application.Abstractions.Services;
using Application.Repositories;
using Application.Users.VerifyEmail;
using Domain.Users;

namespace Application.UnitTests.Users
{
    public sealed class VerifyEmailCommandHandlerTests
    {
        readonly Mock<IUnitOfWork> _unitOfWorkMock;
        readonly Mock<IUserRepository> _userRepositoryMock;
        readonly Mock<IUserTokenGenerator> _tokenGeneratorMock;
        readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
        readonly VerifyEmailCommandValidator _validator = new();

        const string Token = "1234567890123456789012345678901234567890123";
        const string TokenHash = "hashed-token";
        static readonly DateTimeOffset Now = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        static readonly VerifyEmailCommand _validCommand = new(Token);

        public VerifyEmailCommandHandlerTests()
        {
            _unitOfWorkMock = new();
            _userRepositoryMock = new();
            _tokenGeneratorMock = new();
            _dateTimeProviderMock = new();

            _tokenGeneratorMock
                .Setup(x => x.Hash(Token))
                .Returns(TokenHash);
            _dateTimeProviderMock
                .Setup(x => x.UtcNow)
                .Returns(Now);
        }

        [Fact]
        public async Task Handle_InvalidCommand_ReturnsValidationFailure()
        {
            // Arrange
            var command = new VerifyEmailCommand(string.Empty);
            var handler = GetHandler();

            // Act
            Result result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_UserNotFound_ReturnsInvalidEmailVerificationToken()
        {
            // Arrange
            _userRepositoryMock
                .Setup(r => r.GetByEmailVerificationTokenHashAsync(TokenHash, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null!);
            var handler = GetHandler();

            // Act
            Result result = await handler.Handle(_validCommand, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors[0].Code.Should().Be(UserErrors.InvalidEmailVerificationToken.Code);
        }

        [Fact]
        public async Task Handle_UserVerifyEmailReturnsFalse_ReturnsInvalidEmailVerificationToken()
        {
            // Arrange
            var user = User.Create("user@example.com", "hashed-password", RoleId.User);
            _userRepositoryMock
                .Setup(r => r.GetByEmailVerificationTokenHashAsync(TokenHash, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            var handler = GetHandler();

            // Act
            Result result = await handler.Handle(_validCommand, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors[0].Code.Should().Be(UserErrors.InvalidEmailVerificationToken.Code);
        }

        [Fact]
        public async Task Handle_ValidCommand_VerifiesEmailAndSavesChanges()
        {
            // Arrange
            var user = User.Create("user@example.com", "hashed-password", RoleId.User);
            const string tokenHash = "verification-token-hash";
            _ = user.IssueToken(UserTokenPurpose.VerifyEmail, tokenHash, TimeSpan.FromHours(24), Now);

            var command = new VerifyEmailCommand(Token);
            _tokenGeneratorMock
                .Setup(x => x.Hash(Token))
                .Returns(tokenHash);
            _userRepositoryMock
                .Setup(r => r.GetByEmailVerificationTokenHashAsync(tokenHash, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            var handler = GetHandler();

            // Act
            Result result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            user.EmailVerifiedAt.Should().Be(Now);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handle_ValidCommand_CallsTokenGeneratorHash()
        {
            // Arrange
            var user = User.Create("user@example.com", "hashed-password", RoleId.User);
            const string tokenHash = "verification-token-hash";
            _ = user.IssueToken(UserTokenPurpose.VerifyEmail, tokenHash, TimeSpan.FromHours(24), Now);

            _tokenGeneratorMock.Reset();
            _tokenGeneratorMock
                .Setup(x => x.Hash(Token))
                .Returns(tokenHash);

            _userRepositoryMock
                .Setup(r => r.GetByEmailVerificationTokenHashAsync(tokenHash, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            var handler = GetHandler();

            // Act
            Result result = await handler.Handle(_validCommand, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _tokenGeneratorMock.Verify(x => x.Hash(Token), Times.Once);
        }

        VerifyEmailCommandHandler GetHandler() => new(
            _unitOfWorkMock.Object,
            _userRepositoryMock.Object,
            _tokenGeneratorMock.Object,
            _dateTimeProviderMock.Object,
            _validator);
    }
}
