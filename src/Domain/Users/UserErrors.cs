using Common;

namespace Domain.Users
{
    public static class UserErrors
    {
        public static readonly Error EmailTaken =
            Error.Conflict("Users.EmailTaken", "The email is already in use.");
    }
}
