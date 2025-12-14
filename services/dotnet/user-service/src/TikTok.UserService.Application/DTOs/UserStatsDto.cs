namespace TikTok.UserService.Application.DTOs
{
    public class UserStatsDto
    {
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public int VideosCount { get; set; }
        public int TotalLikesReceived { get; set; }
    }
}
