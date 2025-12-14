using AutoMapper;
using TikTok.UserService.Application.DTOs;
using TikTok.UserService.Application.Interfaces;
using TikTok.UserService.Domain.Entities;
using TikTok.UserService.Domain.Interfaces;

namespace TikTok.UserService.Application.Services
{
    public class UserStatsService : IUserStatsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserStatsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserStatsDto?> GetUserStatsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var stats = await _unitOfWork.UserStats.GetByUserIdAsync(userId, cancellationToken);
            return stats != null ? _mapper.Map<UserStatsDto>(stats) : null;
        }

        public async Task<UserStatsDto> CreateDefaultStatsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var stats = new UserStats { UserId = userId };
            await _unitOfWork.UserStats.AddAsync(stats, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return _mapper.Map<UserStatsDto>(stats);
        }

        public async Task UpdateFollowersCountAsync(Guid userId, int delta, CancellationToken cancellationToken = default)
        {
            var stats = await GetOrCreateStatsAsync(userId, cancellationToken);
            stats.FollowersCount = Math.Max(0, stats.FollowersCount + delta);
            await _unitOfWork.UserStats.UpdateAsync(stats, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateFollowingCountAsync(Guid userId, int delta, CancellationToken cancellationToken = default)
        {
            var stats = await GetOrCreateStatsAsync(userId, cancellationToken);
            stats.FollowingCount = Math.Max(0, stats.FollowingCount + delta);
            await _unitOfWork.UserStats.UpdateAsync(stats, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateVideosCountAsync(Guid userId, int delta, CancellationToken cancellationToken = default)
        {
            var stats = await GetOrCreateStatsAsync(userId, cancellationToken);
            stats.VideosCount = Math.Max(0, stats.VideosCount + delta);
            await _unitOfWork.UserStats.UpdateAsync(stats, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateLikesReceivedAsync(Guid userId, int delta, CancellationToken cancellationToken = default)
        {
            var stats = await GetOrCreateStatsAsync(userId, cancellationToken);
            stats.TotalLikesReceived = Math.Max(0, stats.TotalLikesReceived + delta);
            await _unitOfWork.UserStats.UpdateAsync(stats, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateViewsReceivedAsync(Guid userId, int delta, CancellationToken cancellationToken = default)
        {
            var stats = await GetOrCreateStatsAsync(userId, cancellationToken);
            stats.TotalViewsReceived = Math.Max(0, stats.TotalViewsReceived + delta);
            await _unitOfWork.UserStats.UpdateAsync(stats, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task RecalculateStatsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var stats = await GetOrCreateStatsAsync(userId, cancellationToken);

            stats.FollowersCount = await _unitOfWork.Follows.GetFollowersCountAsync(userId, cancellationToken);
            stats.FollowingCount = await _unitOfWork.Follows.GetFollowingCountAsync(userId, cancellationToken);
            stats.LastCalculatedAt = DateTime.UtcNow;

            await _unitOfWork.UserStats.UpdateAsync(stats, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task<UserStats> GetOrCreateStatsAsync(Guid userId, CancellationToken cancellationToken)
        {
            var stats = await _unitOfWork.UserStats.GetByUserIdAsync(userId, cancellationToken);
            if (stats == null)
            {
                stats = new UserStats { UserId = userId };
                await _unitOfWork.UserStats.AddAsync(stats, cancellationToken);
            }
            return stats;
        }
    }
}
