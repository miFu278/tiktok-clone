using TikTok.Shared.Common.Exceptions;
using TikTok.UserService.Application.Interfaces;
using TikTok.UserService.Domain.Entities;
using TikTok.UserService.Domain.Interfaces;

namespace TikTok.UserService.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Role?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Roles.GetByNameAsync(roleName, cancellationToken);
        }

        public async Task<IEnumerable<Role>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Roles.GetUserRolesAsync(userId, cancellationToken);
        }

        public async Task<bool> AssignRoleToUserAsync(Guid userId, string roleName, Guid? assignedBy = null, CancellationToken cancellationToken = default)
        {
            var role = await _unitOfWork.Roles.GetByNameAsync(roleName, cancellationToken);
            if (role == null)
            {
                throw new NotFoundException($"Role '{roleName}' not found");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            // Check if user already has this role
            if (await UserHasRoleAsync(userId, roleName, cancellationToken))
            {
                return false;
            }

            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = role.Id,
                AssignedAt = DateTime.UtcNow,
                AssignedBy = assignedBy
            };

            user.UserRoles.Add(userRole);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> RemoveRoleFromUserAsync(Guid userId, string roleName, CancellationToken cancellationToken = default)
        {
            var role = await _unitOfWork.Roles.GetByNameAsync(roleName, cancellationToken);
            if (role == null)
            {
                return false;
            }

            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                return false;
            }

            var userRole = user.UserRoles.FirstOrDefault(ur => ur.RoleId == role.Id);
            if (userRole == null)
            {
                return false;
            }

            user.UserRoles.Remove(userRole);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> UserHasRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default)
        {
            var userRoles = await GetUserRolesAsync(userId, cancellationToken);
            return userRoles.Any(r => r.Name == roleName);
        }

        public async Task<Role> CreateRoleAsync(string name, string? description = null, CancellationToken cancellationToken = default)
        {
            if (await _unitOfWork.Roles.ExistsByNameAsync(name, cancellationToken))
            {
                throw new ConflictException($"Role '{name}' already exists");
            }

            var role = new Role
            {
                Name = name,
                Description = description,
                IsActive = true
            };

            await _unitOfWork.Roles.AddAsync(role, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return role;
        }
    }
}
