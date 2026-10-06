using Application.Abstractions.Authentication;
using Application.Repositories.Users;
using Common;
using Domain.Users;
using MediatR;

namespace Application.Features.Users
{
    internal sealed class RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher) : IRequestHandler<RegisterUserCommand, Result<Guid>>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;

        public async Task<Result<Guid>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            if (await _userRepository.ExistsByEmailAsync(command.Email))
            {
                return Result<Guid>.Failure(UserErrors.EmailTaken);
            }

            var user = new User
            {
                Id = Guid.CreateVersion7(),
                Email = command.Email,
                Password = _passwordHasher.Hash(command.Password),
                RoleId = Role.UserId
            };

            _userRepository.Add(user);

            await _userRepository.SaveChangesAsync();

            return Result<Guid>.Success(user.Id);
        }
    }
}
