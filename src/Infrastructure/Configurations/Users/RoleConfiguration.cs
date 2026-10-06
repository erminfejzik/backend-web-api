using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Users
{
    public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            if (builder is null) return;

            builder.ToTable("Role", "Users");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(10);

            builder.HasData(
                Role.SuperAdminRole(),
                Role.AdminRole(),
                Role.UserRole()
            );
        }
    }
}
