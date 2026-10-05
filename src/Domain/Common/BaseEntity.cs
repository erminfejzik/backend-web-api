namespace Domain.Common
{
    public abstract class BaseEntity(Guid id)
    {
        public Guid Id { get; protected set; } = id;

        public DateTimeOffset CreatedAt { get; set; }
    }
}
