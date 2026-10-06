using Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Authentication
{
    public sealed class PasswordHasher : IPasswordHasher
    {
        private readonly PasswordHasher<object> hasher = new();

        public string Hash(string password)
            => hasher.HashPassword(null!, password);

        public bool Verify(string password, string hash)
        {
            return hasher.VerifyHashedPassword(null!, hash, password) == PasswordVerificationResult.Success;
        }
    }
}
