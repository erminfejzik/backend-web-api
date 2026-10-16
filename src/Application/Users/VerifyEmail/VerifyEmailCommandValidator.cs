using FluentValidation;

namespace Application.Users.VerifyEmail
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
