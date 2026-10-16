using Application.Abstractions.Authentication;
using Application.Abstractions.Database;
using Application.Extensions;
using Application.Repositories;
using Common;
using Domain.Outbox;
using Domain.Users;
using FluentValidation;
using MediatR;
using System.Text.Json;

namespace Application.Features.Users
{
    internal sealed class RegisterUserCommandHandler(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IOutboxMessageRepository outboxMessageRepository,
        IPasswordHasher passwordHasher,
        IValidator<RegisterUserCommand> validator) 
        : IRequestHandler<RegisterUserCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                return validationResult.ToResultFailure<Guid>();
            }

            if (await userRepository.ExistsByEmailAsync(command.Email, cancellationToken))
            {
                return Result<Guid>.Failure([UserErrors.EmailTaken]);
            }

            var user = User.Create(
                email: command.Email,
                passwordHash: passwordHasher.Hash(command.Password),
                roleId: RoleId.User
            );

            userRepository.Add(user);

            var outboxMessagePayload = new VerificationEmailPayload(user.Id);
            var outboxMessage = OutboxMessage.Create(
                OutboxMessageTypes.VerificationEmail,
                JsonSerializer.Serialize(outboxMessagePayload));

            outboxMessageRepository.Add(outboxMessage);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(user.Id);
        }
    }
}
