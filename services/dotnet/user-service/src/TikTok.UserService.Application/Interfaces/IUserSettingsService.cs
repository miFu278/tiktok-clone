using TikTok.UserService.Application.DTOs;

namespace TikTok.UserService.Application.Interfaces
{
    public interface IUserSettingsService
    {
        Task<UserSettingsDto?> GetUserSettingsAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<UserSettingsDto> UpdateUserSettingsAsync(Guid userId, UserSettingsDto dto, CancellationToken cancellationToken = default);
        Task<UserSettingsDto> CreateDefaultSettingsAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
