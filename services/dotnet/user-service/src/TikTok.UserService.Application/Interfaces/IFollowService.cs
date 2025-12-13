using TikTok.UserService.Application.DTOs;

namespace TikTok.UserService.Application.Interfaces
{
    public interface IFollowService
    {
        Task<FollowResultDto> FollowUserAsync(Guid followerId, Guid followingId, CancellationToken cancellationToken = default);
        Task<FollowResultDto> UnfollowUserAsync(Guid followerId, Guid followingId, CancellationToken cancellationToken = default);
        Task<bool> IsFollowingAsync(Guid followerId, Guid followingId, CancellationToken cancellationToken = default);
        Task<IEnumerable<UserListItemDto>> GetFollowersAsync(Guid userId, int pageNumber, int pageSize, Guid? currentUserId = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<UserListItemDto>> GetFollowingAsync(Guid userId, int pageNumber, int pageSize, Guid? currentUserId = null, CancellationToken cancellationToken = default);
        Task<int> GetFollowersCountAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<int> GetFollowingCountAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
