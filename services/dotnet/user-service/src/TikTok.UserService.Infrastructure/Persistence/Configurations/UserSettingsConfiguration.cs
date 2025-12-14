using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TikTok.UserService.Domain.Entities;

namespace TikTok.UserService.Infrastructure.Persistence.Configurations
{
    public class UserSettingsConfiguration : IEntityTypeConfiguration<UserSettings>
    {
        public void Configure(EntityTypeBuilder<UserSettings> builder)
        {
            builder.ToTable("UserSettings");

            builder.HasKey(s => s.Id);

            builder.HasIndex(s => s.UserId)
                .IsUnique();

            builder.Property(s => s.PreferredLanguage)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(s => s.PreferredContentRegion)
                .IsRequired()
                .HasMaxLength(10);

            builder.HasOne(s => s.User)
                .WithOne(u => u.Settings)
                .HasForeignKey<UserSettings>(s => s.UserId);
        }
    }
}
