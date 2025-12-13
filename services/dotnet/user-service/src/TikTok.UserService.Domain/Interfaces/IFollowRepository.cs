using TikTok.Shared.Common.Abstractions.Repositories;
using TikTok.UserService.Domain.Entities;

namespace TikTok.UserService.Domain.Interfaces
{
    public interface IFollowRepository : IGenericRepository<Follow>
    {
        Task<Follow?> GetFollowAsync(Guid followerId, Guid followingId, CancellationToken cancellationToken = default);
        Task<bool> IsFollowingAsync(Guid followerId, Guid followingId, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetFollowersAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetFollowingAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<int> GetFollowersCountAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<int> GetFollowingCountAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
