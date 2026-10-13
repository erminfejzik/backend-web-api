namespace Domain.Outbox
{
    public static class OutboxMessageTypes
    {
        public const string VerificationEmail = "VerificationEmail";
    }

    public sealed class OutboxMessage
    {
        public Guid Id { get; init; }
        public string Type { get; private set; } = default!;
        /// <summary>
        /// The payload of the outbox message, JSON formatted.
        /// </summary>
        public string Payload { get; private set; } = default!;

        public DateTimeOffset? ProcessedAt { get; private set; }

        public int Attempts { get; set; }
        public DateTimeOffset NextAttemptAt { get; private set; }

        /// <summary>
        /// Optional error message if the processing of the outbox message failed.
        /// Contains the last error message.
        /// </summary>
        public string? Error { get; private set; }

        public DateTimeOffset CreatedAt { get; init; }

        public static OutboxMessage Create(string type, string payload) => new()
        {
            Type = type,
            Payload = payload
        };

        public void MarkProcessed(DateTimeOffset nowUtc)
        {
            ProcessedAt = nowUtc;
            Error = null;
        }

        /// <summary>
        /// Records a failed attempt. Returns true if the message is now finished (gave up),
        /// false if it was scheduled for another retry with exponential backoff.
        /// </summary>
        public bool RegisterFailure(string error, bool permanent, int maxRetries, TimeSpan baseDelay, DateTimeOffset nowUtc)
        {
            Attempts++;

            // Error must be non-empty on failure: it is what distinguishes failed from succeeded.
            error = string.IsNullOrWhiteSpace(error) ? "Unknown error" : error;
            Error = error.Length <= 1000 ? error : error[..1000];

            if (permanent || Attempts > maxRetries)
            {
                ProcessedAt = nowUtc;     // finished; Error != null marks it as failed
                return true;
            }

            NextAttemptAt = nowUtc + baseDelay * Math.Pow(2, Attempts - 1);   // 30s, 60s, 120s
            return false;
        }
    }
}
