using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Users
{
    internal sealed class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
    {
        public void Configure(EntityTypeBuilder<UserToken> builder)
        {
            builder.ToTable("UserToken", "Users");

            builder.HasKey(userToken => userToken.Id);

            builder.HasOne<User>().WithMany().HasForeignKey(x => x.UserId);

            builder.Property(x => x.Purpose).IsRequired().HasConversion<string>().HasMaxLength(30);
            builder.Property(x => x.Email).HasMaxLength(256);
            builder.Property(x => x.TokenHash).HasMaxLength(64).HasColumnType("char(64)");
            builder.Property(x => x.ExpiresAt);
            builder.Property(x => x.UsedAt);
            builder.Property(x => x.CreatedAt);

            builder.HasIndex(x => x.TokenHash).IsUnique();
            builder.HasIndex(x => new { x.UserId, x.Purpose });
        }
    }
}
