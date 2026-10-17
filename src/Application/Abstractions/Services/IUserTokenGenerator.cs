namespace Application.Abstractions.Services
{
    public interface IUserTokenGenerator
    {
        /// <summary>
        /// Creates a new random token. Send the externally; store only the hash.
        /// </summary>
        (string rawToken, string hash) Generate();

        /// <summary>
        /// Hashes a raw token (used when verifying the token).
        /// </summary>
        string Hash(string rawToken);
    }
}
