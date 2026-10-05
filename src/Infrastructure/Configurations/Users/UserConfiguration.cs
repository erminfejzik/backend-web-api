using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Users
{
    public sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            if (builder is null) return;

            builder.ToTable("User", "Users");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Email)
                .IsRequired();

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.Password)
                .IsRequired()
                .HasMaxLength(84); // 84 characters is the maximum length for a hashed password using PasswordHasher

            builder.HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(u => u.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("SYSUTCDATETIME()");

            builder.HasData(new User(
                id: Guid.Parse("01a10cab-98f1-7b35-aeae-96490d14578e"),
                email: "super-admin@example.com",
                password: "AQAAAAIAAYagAAAAEEyQ75ozi8VLY0iYz0IgFd2Jxr/ICs/6nlpojUmIJQ947Sybe428FBlk+Naizm+ZnQ==",
                roleId: Role.SuperAdminId));
        }
    }
}
