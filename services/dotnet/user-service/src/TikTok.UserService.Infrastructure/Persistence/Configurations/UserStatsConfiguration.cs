using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TikTok.UserService.Domain.Entities;

namespace TikTok.UserService.Infrastructure.Persistence.Configurations
{
    public class UserStatsConfiguration : IEntityTypeConfiguration<UserStats>
    {
        public void Configure(EntityTypeBuilder<UserStats> builder)
        {
            builder.ToTable("UserStats");

            builder.HasKey(s => s.Id);

            builder.HasIndex(s => s.UserId)
                .IsUnique();

            builder.HasOne(s => s.User)
                .WithOne(u => u.Stats)
                .HasForeignKey<UserStats>(s => s.UserId);
        }
    }
}
