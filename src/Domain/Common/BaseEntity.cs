namespace Domain.Common
{
    public abstract class BaseEntity(Guid Id, DateTimeOffset CreatedAt)
    {
        public Guid Id { get; protected set; } = Id;

        public DateTimeOffset CreatedAt { get; set; } = CreatedAt;
    }
}
