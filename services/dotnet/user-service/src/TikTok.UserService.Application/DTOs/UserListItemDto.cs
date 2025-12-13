namespace TikTok.UserService.Application.DTOs
{
    public class UserListItemDto
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
        public bool IsFollowing { get; set; }
        public int FollowersCount { get; set; }
    }
}
