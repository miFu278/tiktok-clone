using TikTok.Shared.Common.Abstractions.Repositories;
using TikTok.UserService.Domain.Entities;

namespace TikTok.UserService.Domain.Interfaces
{
    public interface IUserStatsRepository : IGenericRepository<UserStats>
    {
        Task<UserStats?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<UserStats> CreateOrUpdateAsync(UserStats stats, CancellationToken cancellationToken = default);
    }
}
