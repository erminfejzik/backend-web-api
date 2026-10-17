using Application.Users.Register;
using Application.Users.VerifyEmail;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Users
{
    public sealed class UserValidatorTests
    {
        readonly RegisterUserCommandValidator _registerValidator = new();
        readonly VerifyEmailCommandValidator _verifyEmailValidator = new();

        [Theory]
        [InlineData("", TestDisplayName = "Empty email")]
        [InlineData("invalidEmailAddress", TestDisplayName = "Invalid email address")]
        public void RegisterValidator_EmailInvalid_HasEmailError(string email)
        {
            var command = new RegisterUserCommand(email, "examplePassword");

            var result = _registerValidator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Email);
        }

        [Theory]
        [InlineData("", TestDisplayName = "Empty password")]
        [InlineData("short", TestDisplayName = "Short password")]
        [InlineData("mGa973aa?M4!BBQD67EXNp8nAihLqkyPf#iNNYGqhLqkyPf#iNN", TestDisplayName = "Long password")]
        public void RegisterValidator_PasswordInvalid_HasPasswordError(string password)
        {
            var command = new RegisterUserCommand("user@example.com", password);

            var result = _registerValidator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Password);
        }

        [Fact]
        public void RegisterValidator_VaildData_HasNoErrors()
        {
            var command = new RegisterUserCommand("user@example.com", "examplePassword");

            var result = _registerValidator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("", TestDisplayName = "Emptry token")]
        [InlineData("mGa973aa?M4!BBQD67EXNp8nAihLqkyPf#iNNYGqhLqkyPf#iN", TestDisplayName = "Long token")]
        public void VerifyEmailValidator_TokenHashInvalid_HasError(string token)
        {
            var command = new VerifyEmailCommand(token);

            var result = _verifyEmailValidator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.Token);
        }

        [Fact]
        public void VerifyEmailValidator_VaildData_HasNoErrors()
        {
            var command = new VerifyEmailCommand("GC3wl5950TWcwWvUGyAPiPLn-2Rfz1pgU_lzYjMlwfA");

            var result = _verifyEmailValidator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
