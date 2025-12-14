using TikTok.Shared.Common.Abstractions.Entities;

namespace TikTok.UserService.Domain.Entities
{
    public class Role : AuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation property for users in this role
        public ICollection<UserRole> UserRoles { get; set; } = [];
    }
}
