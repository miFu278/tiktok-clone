using AutoMapper;
using TikTok.Shared.Common.Exceptions;
using TikTok.UserService.Application.DTOs;
using TikTok.UserService.Application.Interfaces;
using TikTok.UserService.Domain.Entities;
using TikTok.UserService.Domain.Interfaces;

namespace TikTok.UserService.Application.Services
{
    public class FollowService : IFollowService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FollowService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<FollowResultDto> FollowUserAsync(Guid followerId, Guid followingId, CancellationToken cancellationToken = default)
        {
            if (followerId == followingId)
            {
                throw new BadRequestException("Cannot follow yourself");
            }

            // Check if already following
            if (await _unitOfWork.Follows.IsFollowingAsync(followerId, followingId, cancellationToken))
            {
                throw new ConflictException("Already following this user");
            }

            // Check if users exist
            var follower = await _unitOfWork.Users.GetByIdAsync(followerId, cancellationToken);
            var following = await _unitOfWork.Users.GetByIdAsync(followingId, cancellationToken);

            if (follower == null || following == null)
            {
                throw new NotFoundException("User not found");
            }

            // Create follow relationship
            var follow = new Follow
            {
                FollowerId = followerId,
                FollowingId = followingId,

            };

            await _unitOfWork.Follows.AddAsync(follow, cancellationToken);

            // Update stats
            var followerStats = await _unitOfWork.UserStats.GetByUserIdAsync(followerId, cancellationToken);
            var followingStats = await _unitOfWork.UserStats.GetByUserIdAsync(followingId, cancellationToken);

            if (followerStats != null) followerStats.IncrementFollowing();
            if (followingStats != null) followingStats.IncrementFollowers();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new FollowResultDto
            {
                IsFollowing = true,
                FollowersCount = followingStats?.FollowersCount ?? 0
            };
        }

        public async Task<FollowResultDto> UnfollowUserAsync(Guid followerId, Guid followingId, CancellationToken cancellationToken = default)
        {
            var follow = await _unitOfWork.Follows.GetFollowAsync(followerId, followingId, cancellationToken);

            if (follow == null)
            {
                throw new NotFoundException("Follow relationship not found");
            }

            await _unitOfWork.Follows.DeleteAsync(follow.Id, cancellationToken);

            // Update stats
            var followerStats = await _unitOfWork.UserStats.GetByUserIdAsync(followerId, cancellationToken);
            var followingStats = await _unitOfWork.UserStats.GetByUserIdAsync(followingId, cancellationToken);

            if (followerStats != null) followerStats.DecrementFollowing();
            if (followingStats != null) followingStats.DecrementFollowers();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new FollowResultDto
            {
                IsFollowing = false,
                FollowersCount = followingStats?.FollowersCount ?? 0
            };
        }

        public async Task<bool> IsFollowingAsync(Guid followerId, Guid followingId, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Follows.IsFollowingAsync(followerId, followingId, cancellationToken);
        }

        public async Task<IEnumerable<UserListItemDto>> GetFollowersAsync(Guid userId, int pageNumber, int pageSize, Guid? currentUserId = null, CancellationToken cancellationToken = default)
        {
            var followers = await _unitOfWork.Follows.GetFollowersAsync(userId, pageNumber, pageSize, cancellationToken);
            var followerList = _mapper.Map<IEnumerable<UserListItemDto>>(followers);

            if (currentUserId.HasValue)
            {
                foreach (var follower in followerList)
                {
                    follower.IsFollowing = await _unitOfWork.Follows.IsFollowingAsync(currentUserId.Value, follower.Id, cancellationToken);
                }
            }

            return followerList;
        }

        public async Task<IEnumerable<UserListItemDto>> GetFollowingAsync(Guid userId, int pageNumber, int pageSize, Guid? currentUserId = null, CancellationToken cancellationToken = default)
        {
            var following = await _unitOfWork.Follows.GetFollowingAsync(userId, pageNumber, pageSize, cancellationToken);
            var followingList = _mapper.Map<IEnumerable<UserListItemDto>>(following);

            if (currentUserId.HasValue)
            {
                foreach (var user in followingList)
                {
                    user.IsFollowing = await _unitOfWork.Follows.IsFollowingAsync(currentUserId.Value, user.Id, cancellationToken);
                }
            }

            return followingList;
        }

        public async Task<int> GetFollowersCountAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Follows.GetFollowersCountAsync(userId, cancellationToken);
        }

        public async Task<int> GetFollowingCountAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Follows.GetFollowingCountAsync(userId, cancellationToken);
        }
    }
}
