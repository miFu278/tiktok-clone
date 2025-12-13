using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TikTok.UserService.Domain.Entities;

namespace TikTok.UserService.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(u => u.Email)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            builder.Property(u => u.UserName)
                .HasMaxLength(30);

            builder.HasIndex(u => u.UserName)
                .IsUnique()
                .HasFilter("[UserName] IS NOT NULL AND [IsDeleted] = 0");

            builder.Property(u => u.PasswordHash)
                .HasMaxLength(500);

            builder.Property(u => u.FullName)
                .HasMaxLength(100);

            builder.Property(u => u.Bio)
                .HasMaxLength(500);

            builder.Property(u => u.AvatarUrl)
                .HasMaxLength(500);

            builder.Property(u => u.Gender)
                .IsRequired();

            builder.Property(u => u.EmailVerificationToken)
                .HasMaxLength(500);

            builder.Property(u => u.PasswordResetToken)
                .HasMaxLength(500);

            // Relationships
            builder.HasOne(u => u.Settings)
                .WithOne(s => s.User)
                .HasForeignKey<UserSettings>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(u => u.Stats)
                .WithOne(s => s.User)
                .HasForeignKey<UserStats>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Followers)
                .WithOne(f => f.Following)
                .HasForeignKey(f => f.FollowingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Following)
                .WithOne(f => f.Follower)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Query filters
            builder.HasQueryFilter(u => !u.IsDeleted);
        }
    }
}
