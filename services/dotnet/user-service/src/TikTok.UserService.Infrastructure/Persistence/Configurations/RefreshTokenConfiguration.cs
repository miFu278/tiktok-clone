using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TikTok.UserService.Domain.Entities;

namespace TikTok.UserService.Infrastructure.Persistence.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(rt => rt.Id);

            builder.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasIndex(rt => rt.Token)
                .IsUnique();

            builder.Property(rt => rt.CreatedByIp)
                .HasMaxLength(50);

            builder.Property(rt => rt.RevokedByIp)
                .HasMaxLength(50);

            builder.Property(rt => rt.ReplacedByToken)
                .HasMaxLength(500);

            builder.Property(rt => rt.ReasonRevoked)
                .HasMaxLength(200);

            builder.HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index for performance
            builder.HasIndex(rt => rt.UserId);
            builder.HasIndex(rt => rt.ExpiresAt);
        }
    }
}
