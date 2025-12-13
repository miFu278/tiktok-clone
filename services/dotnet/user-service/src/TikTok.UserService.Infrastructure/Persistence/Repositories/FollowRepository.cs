using Microsoft.EntityFrameworkCore;
using TikTok.UserService.Domain.Entities;
using TikTok.UserService.Domain.Interfaces;

namespace TikTok.UserService.Infrastructure.Persistence.Repositories
{
    public class FollowRepository : GenericRepository<Follow>, IFollowRepository
    {
        public FollowRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Follow?> GetFollowAsync(Guid followerId, Guid followingId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId, cancellationToken);
        }

        public async Task<bool> IsFollowingAsync(Guid followerId, Guid followingId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AnyAsync(f => f.FollowerId == followerId && f.FollowingId == followingId, cancellationToken);
        }

        public async Task<IEnumerable<User>> GetFollowersAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(f => f.FollowingId == userId)
                .Include(f => f.Follower)
                    .ThenInclude(u => u.Stats)
                .Select(f => f.Follower)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<User>> GetFollowingAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(f => f.FollowerId == userId)
                .Include(f => f.Following)
                    .ThenInclude(u => u.Stats)
                .Select(f => f.Following)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetFollowersCountAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .CountAsync(f => f.FollowingId == userId, cancellationToken);
        }

        public async Task<int> GetFollowingCountAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .CountAsync(f => f.FollowerId == userId, cancellationToken);
        }
    }
}
