using Application.Abstractions.Database;
using Domain.Common;
using Domain.Outbox;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IUnitOfWork, IReadDbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<UserToken> UserTokens => Set<UserToken>();
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        IQueryable<User> IReadDbContext.Users => Users.AsNoTracking();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ArgumentNullException.ThrowIfNull(modelBuilder);

            base.OnModelCreating(modelBuilder);

            // Scans the assembly and applies all IEntityTypeConfiguration classes
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Apply NEWSEQUENTIALID() to Guid primary keys
                var primaryKey = entityType.FindPrimaryKey();
                if (primaryKey is not null)
                {
                    foreach (var property in primaryKey.Properties)
                    {
                        // Apply it only to Guid primary keys
                        if (property.ClrType == typeof(Guid))
                        {
                            // Tells EF Core to generate the SQL Server-optimized UUIDv7 on the client side
                            property.SetValueGeneratorFactory((_, _) => new SqlServerUuidV7Generator());
                        }
                    }
                }

                // Apply SYSUTCDATETIME() to CreatedAt property of BaseEntity
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType) && entityType.BaseType is null)
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property(nameof(BaseEntity.CreatedAt))
                        .HasDefaultValueSql("SYSUTCDATETIME()");
                }
            }
        }
    }
}
