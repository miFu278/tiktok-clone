namespace TikTok.UserService.Infrastructure.ExternalServices
{
    public class BrevoEmailOptions
    {
        public const string SectionName = "Brevo";

        public string ApiKey { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://app.tiktok-clone.com";
    }
}
