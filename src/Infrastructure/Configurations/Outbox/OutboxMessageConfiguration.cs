using Domain.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Outbox
{
    internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("OutboxMessages", "Outbox");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Type).HasMaxLength(100);
            builder.Property(m => m.Payload).HasMaxLength(1000);
            builder.Property(m => m.Error).HasMaxLength(1000);
            builder.Property(x => x.NextAttemptAt).HasDefaultValueSql("SYSUTCDATETIME()");
            builder.HasIndex(m => new { m.ProcessedAt, m.NextAttemptAt });
        }
    }
}
