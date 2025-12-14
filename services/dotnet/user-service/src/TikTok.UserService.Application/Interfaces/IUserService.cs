using TikTok.UserService.Application.DTOs;
using TikTok.UserService.Domain.ValueObjects;

namespace TikTok.UserService.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<UserDto?> GetUserByEmailAsync(Email email, CancellationToken cancellationToken = default);
        Task<UserDto?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<UserProfileDto?> GetUserProfileAsync(Guid userId, Guid? currentUserId = null, CancellationToken cancellationToken = default);
        Task<UserDto> UpdateProfileAsync(Guid userId, UpdateProfileDto dto, CancellationToken cancellationToken = default);
        Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAccountAsync(Guid userId, string password, CancellationToken cancellationToken = default);
        Task<IEnumerable<UserListItemDto>> SearchUsersAsync(string searchTerm, int pageNumber, int pageSize, Guid? currentUserId = null, CancellationToken cancellationToken = default);
    }
}
