namespace TikTok.UserService.Domain.Entities
{
    public class Role
    {
        public Guid RoleId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }

        // Navigation property for users in this role
        public ICollection<UserRole> UserRoles { get; set; } = [];
    }
}
