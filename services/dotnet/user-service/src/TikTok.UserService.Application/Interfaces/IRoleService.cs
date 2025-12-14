using TikTok.UserService.Domain.Entities;

namespace TikTok.UserService.Application.Interfaces
{
    public interface IRoleService
    {
        Task<Role?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default);
        Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<bool> AssignRoleToUserAsync(Guid userId, string roleName, Guid? assignedBy = null, CancellationToken cancellationToken = default);
        Task<bool> RemoveRoleFromUserAsync(Guid userId, string roleName, CancellationToken cancellationToken = default);
        Task<bool> UserHasRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default);
        Task<Role> CreateRoleAsync(string name, string? description = null, CancellationToken cancellationToken = default);
    }
}
