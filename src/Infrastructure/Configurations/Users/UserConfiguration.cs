using Domain.Users;
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

            builder.Property(u => u.Email).IsRequired();

            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(84); // 84 characters is the maximum length for a hashed password using PasswordHasher

            builder.HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(user => user.IsBlocked).IsRequired();

            builder.Property(user => user.TwoFactorEnabled).IsRequired();

            builder.Property(user => user.EmailVerifiedAt).IsRequired(false);

            builder.Property(user => user.LastLoginAt).IsRequired(false);

            builder.Property(user => user.FailedLoginAttempts).IsRequired();

            builder.Property(user => user.LockoutEnd).IsRequired(false);

            builder.HasMany(u => u.Tokens)
                .WithOne()
                .HasForeignKey(t => t.UserId);
        }
    }
}
