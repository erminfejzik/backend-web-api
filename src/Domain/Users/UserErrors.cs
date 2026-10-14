using Common;

namespace Domain.Users
{
    public static class UserErrors
    {
        public static readonly Error EmailTaken =
            Error.Conflict("Users.EmailTaken", "The email is already in use.");

        public static readonly Error InvalidEmailVerificationToken =
            Error.Validation("Users.InvalidEmailVerificationToken", "The email verification token is invalid or has expired.");
    }
}
