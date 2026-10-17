namespace Domain.Outbox
{
    /// <summary>
    /// Payload for sending a verification email to a user.
    /// </summary>
    /// <param name="UserId"></param>
    public sealed record VerificationEmailPayload(Guid UserId);
}
