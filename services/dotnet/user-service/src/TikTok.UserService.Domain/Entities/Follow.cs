using TikTok.Shared.Common.Abstractions.Entities;

namespace TikTok.UserService.Domain.Entities
{
    public class Follow : AuditableEntity
    {
        public Guid FollowerId { get; set; }
        public Guid FollowingId { get; set; }

        // Navigation Properties
        public User Follower { get; set; } = null!;
        public User Following { get; set; } = null!;
    }
}
