namespace Domain.Outbox
{
    public static class OutboxMessageTypes
    {
        public const string VerificationEmail = "VerificationEmail";
    }

    public sealed class OutboxMessage
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = default!;
        /// <summary>
        /// The payload of the outbox message, JSON formatted.
        /// </summary>
        public string Payload { get; set; } = default!;

        public DateTimeOffset? ProcessedAt { get; set; }

        public int Attempts { get; set; }
        public DateTimeOffset NextAttemptAt { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Optional error message if the processing of the outbox message failed.
        /// Contains the last error message.
        /// </summary>
        public string? Error { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
