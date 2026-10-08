using Common;

namespace Infrastructure.Time
{
    internal sealed class DateTimeProvider(TimeProvider time) : IDateTimeProvider
    {
        public DateTimeOffset UtcNow => time.GetUtcNow();
    }
}
