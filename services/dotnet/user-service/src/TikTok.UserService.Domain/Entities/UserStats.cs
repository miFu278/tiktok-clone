using TikTok.Shared.Common.Abstractions.Entities;

namespace TikTok.UserService.Domain.Entities
{
    public class UserStats : AuditableEntity
    {
        public Guid UserId { get; set; }

        // Follower Statistics
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }

        // Content Statistics
        public int VideosCount { get; set; }
        public int TotalLikesReceived { get; set; }
        public int TotalViewsReceived { get; set; }
        public int TotalCommentsReceived { get; set; }
        public int TotalSharesReceived { get; set; }

        // Engagement Statistics
        public int TotalLikesGiven { get; set; }
        public int TotalCommentsGiven { get; set; }
        public int TotalSharesGiven { get; set; }

        // Last Updated
        public DateTime LastCalculatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Property
        public User User { get; set; } = null!;

        // Helper Methods
        public void IncrementFollowers() => FollowersCount++;
        public void DecrementFollowers() => FollowersCount = Math.Max(0, FollowersCount - 1);
        public void IncrementFollowing() => FollowingCount++;
        public void DecrementFollowing() => FollowingCount = Math.Max(0, FollowingCount - 1);
        public void IncrementVideos() => VideosCount++;
        public void DecrementVideos() => VideosCount = Math.Max(0, VideosCount - 1);
    }
}
