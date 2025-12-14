using TikTok.UserService.Domain.Enums;

namespace TikTok.UserService.Application.DTOs
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public Gender Gender { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsFollowing { get; set; }
        public bool IsFollowedBy { get; set; }
        public UserStatsDto Stats { get; set; } = new();
    }
}
