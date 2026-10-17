using Application.Abstractions.Services;
using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services
{
    internal sealed class UserTokenGenerator : IUserTokenGenerator
    {
        private const int TokenBytes = 32;   // 256 bits of entropy

        public (string rawToken, string hash) Generate()
        {
            // Cryptographically secure random bytes, URL-safe so the token can go straight into a link.
            var raw = Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(TokenBytes));
            return (raw, Hash(raw));
        }

        public string Hash(string rawToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(rawToken);

            // Plain SHA-256 is correct here: the token is 256 bits of randomness, so there is nothing
            // to brute-force and no need for a salt or a slow hash (unlike passwords).
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
            return Convert.ToHexString(bytes);   // 64 chars -> use HasMaxLength(64) in EF config
        }
    }
}
