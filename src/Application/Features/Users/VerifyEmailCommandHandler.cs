using Application.Abstractions.Database;
using Application.Abstractions.Services;
using Application.Extensions;
using Application.Repositories;
using Common;
using Domain.Users;
using FluentValidation;
using MediatR;

namespace Application.Features.Users
{
    internal sealed class VerifyEmailCommandHandler(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IUserTokenGenerator tokenGenerator,
        IDateTimeProvider dateTimeProvider,
        IValidator<VerifyEmailCommand> validator)
        : IRequestHandler<VerifyEmailCommand, Result>
    {
        public async Task<Result> Handle(VerifyEmailCommand command, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                return validationResult.ToResultFailure();
            }

            var tokenHash = tokenGenerator.Hash(command.Token);
            var user = await userRepository.GetByEmailVerificationTokenHashAsync(tokenHash, cancellationToken);

            if (user is null || !user.VerifyEmail(tokenHash, dateTimeProvider.UtcNow))
            {
                return Result.Failure([UserErrors.InvalidEmailVerificationToken]);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
