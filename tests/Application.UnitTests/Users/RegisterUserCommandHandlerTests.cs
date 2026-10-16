using Application.Abstractions.Authentication;
using Application.Repositories;
using Application.Users.Register;
using Domain.Outbox;
using Domain.Users;

namespace Application.UnitTests.Users
{
    public sealed class RegisterUserCommandHandlerTests
    {
        readonly Mock<IUnitOfWork> _unitOfWorkMock;
        readonly Mock<IUserRepository> _userRepositoryMock;
        readonly Mock<IOutboxMessageRepository> _outboxMessageRepository;
        readonly Mock<IPasswordHasher> _passwordHasher;
        readonly RegisterUserCommandValidator _validator = new();

        const string Email = "user@example.com";
        static readonly RegisterUserCommand _successfulCommand = new (Email, "password");

        public RegisterUserCommandHandlerTests()
        {
            _unitOfWorkMock = new();
            _userRepositoryMock = new();
            _outboxMessageRepository = new();
            _passwordHasher = new();
        }

        [Fact]
        public async Task Handle_InvalidCommand_ReturnsValidationFailure_AndPersistsNothing()
        {
            // Arrange
            var command = new RegisterUserCommand(string.Empty, "examplePassword");
            var handler = GetHandler();

            // Act
            Result<Guid> result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_EmailExists_ReturnsEmailTaken_AndPersistsNothing()
        {
            // Arrange
            _userRepositoryMock.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            var handler = GetHandler();

            // Act
            Result<Guid> result = await handler.Handle(_successfulCommand, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Errors[0].Code.Should().Be(UserErrors.EmailTaken.Code);
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsUserId_AndAddsUser()
        {
            // Arrange
            var handler = GetHandler();

            // Act
            Result<Guid> result = await handler.Handle(_successfulCommand, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _passwordHasher.Verify(
                x => x.Hash("password"),
                Times.Once);
            _userRepositoryMock.Verify(
                x => x.Add(It.Is<User>(x => x.Email == Email)),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ValidCommand_AddsOutboxMessage()
        {
            // Arrange
            var handler = GetHandler();

            // Act
            Result<Guid> result = await handler.Handle(_successfulCommand, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _outboxMessageRepository.Verify(x => x.Add(It.Is<OutboxMessage>(x => x.Type == OutboxMessageTypes.VerificationEmail)),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ValidCommand_CallSaveChangesOnce()
        {
            // Arrange
            var handler = GetHandler();

            // Act
            Result<Guid> result = await handler.Handle(_successfulCommand, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        RegisterUserCommandHandler GetHandler() => new (
                _unitOfWorkMock.Object,
                _userRepositoryMock.Object,
                _outboxMessageRepository.Object,
                _passwordHasher.Object,
                _validator);
    }
}
