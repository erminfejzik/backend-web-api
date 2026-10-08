using Application.Abstractions.Authentication;
using Application.Abstractions.Services;
using Application.Extensions;
using Application.Repositories.Users;
using Common;
using Domain.Users;
using FluentValidation;
using MediatR;

namespace Application.Features.Users
{
    internal sealed class RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IValidator<RegisterUserCommand> validator,
        IEmailService emailService) 
        : IRequestHandler<RegisterUserCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                return validationResult.ToResultFailure<Guid>();
            }

            if (await userRepository.ExistsByEmailAsync(command.Email))
            {
                return Result<Guid>.Failure([UserErrors.EmailTaken]);
            }

            var user = new User
            {
                Email = command.Email,
                Password = passwordHasher.Hash(command.Password),
                RoleId = Role.UserId
            };

            userRepository.Add(user);

            await userRepository.SaveChangesAsync();

            await emailService.SendAsync(command.Email, "Welcome to Our App", "<p>Thank you for registering!</p>", cancellationToken);

            return Result<Guid>.Success(user.Id);
        }
    }
}
