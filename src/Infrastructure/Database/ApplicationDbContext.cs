using Application.Abstractions.Database;
using Domain.Outbox;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IUnitOfWork, IReadDbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserToken> UserTokens => Set<UserToken>();
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        IQueryable<User> IReadDbContext.Users => Users.AsNoTracking();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ArgumentNullException.ThrowIfNull(modelBuilder);

            base.OnModelCreating(modelBuilder);

            // Scans the assembly and applies all IEntityTypeConfiguration classes
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // 2. Loop through the final compiled model and apply NEWSEQUENTIALID() globally
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var primaryKey = entityType.FindPrimaryKey();
                if (primaryKey == null) continue;

                foreach (var property in primaryKey.Properties)
                {
                    // Apply it only to Guid primary keys
                    if (property.ClrType == typeof(Guid))
                    {
                        property.SetDefaultValueSql("NEWSEQUENTIALID()");
                    }
                }
            }
        }
    }
}
