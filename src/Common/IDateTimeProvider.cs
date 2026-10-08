namespace Common
{
    public interface IDateTimeProvider
    {
        DateTimeOffset UtcNow { get; }
    }
}
