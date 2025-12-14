using AutoMapper;
using TikTok.Shared.Common.Exceptions;
using TikTok.Shared.Security.Interfaces;
using TikTok.UserService.Application.DTOs;
using TikTok.UserService.Application.Interfaces;
using TikTok.UserService.Domain.Interfaces;
using TikTok.UserService.Domain.ValueObjects;

namespace TikTok.UserService.Application.Services
{
    public class UserService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IPasswordHasher passwordHasher
        )
        : IUserService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;

        public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto dto, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken)
                ?? throw new NotFoundException("User not found.");

            // Verify current password (correct order: plaintext, hashed)
            if (!_passwordHasher.VerifyPassword(dto.CurrentPassword, user.PasswordHash ?? string.Empty))
            {
                throw new UnauthorizedException("Current password is incorrect.");
            }

            // Validate new password is different
            if (_passwordHasher.VerifyPassword(dto.NewPassword, user.PasswordHash ?? string.Empty))
            {
                throw new BadRequestException("New password must be different from the current password.");
            }

            // Update password
            user.PasswordHash = _passwordHasher.HashPassword(dto.NewPassword);
            user.SetUpdated(userId);

            await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAccountAsync(Guid userId, string password, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken)
                ?? throw new NotFoundException("User not found.");

            if (!_passwordHasher.VerifyPassword(password, user.PasswordHash ?? string.Empty))
            {
                throw new UnauthorizedException("Password is incorrect.");
            }

            // Soft delete the user account
            user.SoftDelete(userId);
            await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<UserDto?> GetUserByEmailAsync(Email email, CancellationToken cancellationToken = default)
            => await _unitOfWork.Users.GetByEmailAsync(email, cancellationToken)
                is { } user
                ? _mapper.Map<UserDto>(user)
                : null;

        public async Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
            => await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken)
                is { } user
                ? _mapper.Map<UserDto>(user)
                : null;

        public async Task<UserDto?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
            => await _unitOfWork.Users.GetByUsernameAsync(username, cancellationToken)
                is { } user
                ? _mapper.Map<UserDto>(user)
                : null;

        public async Task<UserProfileDto?> GetUserProfileAsync(Guid userId, Guid? currentUserId = null, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
            if (user == null) return null;

            var profile = _mapper.Map<UserProfileDto>(user);

            // Check follow status if currentUserId is provided
            if (currentUserId.HasValue && currentUserId.Value != userId)
            {
                profile.IsFollowing = await _unitOfWork.Follows.IsFollowingAsync(currentUserId.Value, userId, cancellationToken);
                profile.IsFollowedBy = await _unitOfWork.Follows.IsFollowingAsync(userId, currentUserId.Value, cancellationToken);
            }

            return profile;
        }
        public async Task<IEnumerable<UserListItemDto>> SearchUsersAsync(string searchTerm, int pageNumber, int pageSize, Guid? currentUserId = null, CancellationToken cancellationToken = default)
        {
            var users = await _unitOfWork.Users.SearchUsersAsync(searchTerm, pageNumber, pageSize, cancellationToken);
            var userList = _mapper.Map<IEnumerable<UserListItemDto>>(users);

            // Check follow status for each user if currentUserId is provided
            if (currentUserId.HasValue)
            {
                foreach (var userItem in userList)
                {
                    userItem.IsFollowing = await _unitOfWork.Follows.IsFollowingAsync(currentUserId.Value, userItem.Id, cancellationToken);
                }
            }

            return userList;
        }

        public async Task<UserDto> UpdateProfileAsync(Guid userId, UpdateProfileDto dto, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            if (!string.IsNullOrWhiteSpace(dto.UserName) && dto.UserName != user.UserName)
            {
                var existingUser = await _unitOfWork.Users.GetByUsernameAsync(dto.UserName, cancellationToken);
                if (existingUser != null)
                {
                    throw new ConflictException("Username already taken");
                }

                user.UserName = dto.UserName;
            }

            if (dto.FullName != null)
            {
                user.FullName = dto.FullName;
            }
            if (dto.DateOfBirth != null)
            {
                user.DateOfBirth = (DateTime)dto.DateOfBirth;
            }
            if (dto.AvatarUrl != null)
            {
                user.AvatarUrl = dto.AvatarUrl;
            }
            if (dto.Bio != null)
            {
                user.Bio = dto.Bio;
            }

            user.SetUpdated(userId);

            await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<UserDto>(user);
        }
    }
}
