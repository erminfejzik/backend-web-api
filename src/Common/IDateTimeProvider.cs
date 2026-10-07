namespace Common
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
    }
}
