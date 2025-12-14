using TikTok.UserService.Application.DTOs;

namespace TikTok.UserService.Application.Interfaces
{
    public interface IUserStatsService
    {
        Task<UserStatsDto?> GetUserStatsAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<UserStatsDto> CreateDefaultStatsAsync(Guid userId, CancellationToken cancellationToken = default);
        Task UpdateFollowersCountAsync(Guid userId, int delta, CancellationToken cancellationToken = default);
        Task UpdateFollowingCountAsync(Guid userId, int delta, CancellationToken cancellationToken = default);
        Task UpdateVideosCountAsync(Guid userId, int delta, CancellationToken cancellationToken = default);
        Task UpdateLikesReceivedAsync(Guid userId, int delta, CancellationToken cancellationToken = default);
        Task UpdateViewsReceivedAsync(Guid userId, int delta, CancellationToken cancellationToken = default);
        Task RecalculateStatsAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
