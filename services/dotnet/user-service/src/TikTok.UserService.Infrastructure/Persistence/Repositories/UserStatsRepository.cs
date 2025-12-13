using Microsoft.EntityFrameworkCore;
using TikTok.UserService.Domain.Entities;
using TikTok.UserService.Domain.Interfaces;

namespace TikTok.UserService.Infrastructure.Persistence.Repositories
{
    public class UserStatsRepository : GenericRepository<UserStats>, IUserStatsRepository
    {
        public UserStatsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<UserStats?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
        }

        public async Task<UserStats> CreateOrUpdateAsync(UserStats stats, CancellationToken cancellationToken = default)
        {
            var existing = await GetByUserIdAsync(stats.UserId, cancellationToken);

            if (existing == null)
            {
                await _dbSet.AddAsync(stats, cancellationToken);
                return stats;
            }

            existing.FollowersCount = stats.FollowersCount;
            existing.FollowingCount = stats.FollowingCount;
            existing.VideosCount = stats.VideosCount;
            existing.TotalLikesReceived = stats.TotalLikesReceived;
            existing.TotalViewsReceived = stats.TotalViewsReceived;
            existing.TotalCommentsReceived = stats.TotalCommentsReceived;
            existing.TotalSharesReceived = stats.TotalSharesReceived;
            existing.TotalLikesGiven = stats.TotalLikesGiven;
            existing.TotalCommentsGiven = stats.TotalCommentsGiven;
            existing.TotalSharesGiven = stats.TotalSharesGiven;
            existing.LastCalculatedAt = DateTime.UtcNow;

            _dbSet.Update(existing);
            return existing;
        }
    }
}
