namespace TikTok.UserService.Application.DTOs
{
    public class UserSettingsDto
    {
        // Privacy Settings
        public bool IsPrivateAccount { get; set; }
        public bool AllowComments { get; set; }
        public bool AllowDuet { get; set; }
        public bool AllowStitch { get; set; }
        public bool AllowDownload { get; set; }

        // Notification Settings
        public bool PushNotificationsEnabled { get; set; }
        public bool EmailNotificationsEnabled { get; set; }
        public bool NotifyOnLikes { get; set; }
        public bool NotifyOnComments { get; set; }
        public bool NotifyOnFollows { get; set; }
        public bool NotifyOnMentions { get; set; }

        // Content Preferences
        public string PreferredLanguage { get; set; } = "en";
        public string PreferredContentRegion { get; set; } = "US";
    }
}
