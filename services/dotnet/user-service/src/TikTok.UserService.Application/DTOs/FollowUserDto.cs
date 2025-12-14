namespace TikTok.UserService.Application.DTOs
{
    public class FollowUserDto
    {
        public Guid UserId { get; set; }
    }

    public class FollowResultDto
    {
        public bool IsFollowing { get; set; }
        public int FollowersCount { get; set; }
    }
}
