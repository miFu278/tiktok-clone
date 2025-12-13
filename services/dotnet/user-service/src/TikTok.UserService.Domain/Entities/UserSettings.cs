using TikTok.Shared.Common.Abstractions.Entities;

namespace TikTok.UserService.Domain.Entities
{
    public class UserSettings : AuditableEntity
    {
        public Guid UserId { get; set; }

        // Privacy Settings
        public bool IsPrivateAccount { get; set; }
        public bool AllowComments { get; set; } = true;
        public bool AllowDuet { get; set; } = true;
        public bool AllowStitch { get; set; } = true;
        public bool AllowDownload { get; set; } = true;

        // Notification Settings
        public bool PushNotificationsEnabled { get; set; } = true;
        public bool EmailNotificationsEnabled { get; set; } = true;
        public bool NotifyOnLikes { get; set; } = true;
        public bool NotifyOnComments { get; set; } = true;
        public bool NotifyOnFollows { get; set; } = true;
        public bool NotifyOnMentions { get; set; } = true;

        // Content Preferences
        public string PreferredLanguage { get; set; } = "en";
        public string PreferredContentRegion { get; set; } = "US";

        // Navigation Property
        public User User { get; set; } = null!;
    }
}
