using FluentValidation;

namespace Application.Features.Users
{
    internal sealed class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
    {
        public VerifyEmailCommandValidator()
        {
            RuleFor(command => command.Token)
                .NotEmpty().WithMessage("Verification token is required.")
                .MaximumLength(64);
        }
    }
}
