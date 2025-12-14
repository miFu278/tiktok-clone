using Grpc.Core;
using TikTok.UserService.API.Mappers;
using TikTok.UserService.Application.DTOs;
using TikTok.UserService.Application.Interfaces;
using TikTok.Shared.Protos.UserService;
using TikTok.Shared.Protos;

namespace TikTok.UserService.API.Services
{
    public class UserGrpcService : TikTok.Shared.Protos.UserService.UserService.UserServiceBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IFollowService _followService;
        private readonly IUserSettingsService _userSettingsService;
        private readonly ILogger<UserGrpcService> _logger;

        public UserGrpcService(
            IAuthService authService,
            IUserService userService,
            IFollowService followService,
            IUserSettingsService userSettingsService,
            ILogger<UserGrpcService> logger)
        {
            _authService = authService;
            _userService = userService;
            _followService = followService;
            _userSettingsService = userSettingsService;
            _logger = logger;
        }

        #region Authentication

        public override async Task<UserResponse> Register(RegisterRequest request, ServerCallContext context)
        {
            try
            {
                var dto = request.ToDto();
                var user = await _authService.RegisterAsync(dto, context.CancellationToken);
                return new UserResponse { User = user.ToProto() };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
        {
            try
            {
                var dto = request.ToDto();
                var response = await _authService.LoginAsync(dto, context.CancellationToken);

                return new LoginResponse
                {
                    AccessToken = response.AccessToken,
                    RefreshToken = response.RefreshToken,
                    ExpiresAt = response.ExpiresAt.ToString("O"),
                    User = response.User.ToProto()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging in");
                throw new RpcException(new Status(StatusCode.Unauthenticated, ex.Message));
            }
        }

        public override async Task<LoginResponse> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
        {
            try
            {
                var dto = new RefreshTokenDto { RefreshToken = request.RefreshToken };
                var response = await _authService.RefreshTokenAsync(dto, context.CancellationToken);

                return new LoginResponse
                {
                    AccessToken = response.AccessToken,
                    RefreshToken = response.RefreshToken,
                    ExpiresAt = response.ExpiresAt.ToString("O"),
                    User = response.User.ToProto()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                throw new RpcException(new Status(StatusCode.Unauthenticated, ex.Message));
            }
        }

        public override async Task<Empty> Logout(LogoutRequest request, ServerCallContext context)
        {
            try
            {
                await _authService.LogoutAsync(request.RefreshToken, context.CancellationToken);
                return new Empty();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging out");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override async Task<VerifyEmailResponse> VerifyEmail(VerifyEmailRequest request, ServerCallContext context)
        {
            try
            {
                var dto = new VerifyEmailDto { Token = request.Token };
                var success = await _authService.VerifyEmailAsync(dto, context.CancellationToken);
                return new VerifyEmailResponse { Success = success };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying email");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override async Task<Empty> ForgotPassword(ForgotPasswordRequest request, ServerCallContext context)
        {
            try
            {
                var dto = new ForgotPasswordDto { Email = request.Email };
                await _authService.ForgotPasswordAsync(dto, context.CancellationToken);
                return new Empty();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in forgot password");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override async Task<Empty> ResetPassword(ResetPasswordRequest request, ServerCallContext context)
        {
            try
            {
                var dto = new ResetPasswordDto
                {
                    Token = request.Token,
                    NewPassword = request.NewPassword,
                    ConfirmNewPassword = request.ConfirmNewPassword
                };
                await _authService.ResetPasswordAsync(dto, context.CancellationToken);
                return new Empty();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        #endregion

        #region User Management

        public override async Task<UserResponse> GetUser(GetUserRequest request, ServerCallContext context)
        {
            try
            {
                var userId = Guid.Parse(request.UserId);
                var user = await _userService.GetUserByIdAsync(userId, context.CancellationToken);

                if (user == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
                }

                return new UserResponse { User = user.ToProto() };
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override async Task<UserProfileResponse> GetUserProfile(GetUserProfileRequest request, ServerCallContext context)
        {
            try
            {
                var userId = Guid.Parse(request.UserId);
                var currentUserId = string.IsNullOrEmpty(request.CurrentUserId)
                    ? (Guid?)null
                    : Guid.Parse(request.CurrentUserId);

                var profile = await _userService.GetUserProfileAsync(userId, currentUserId, context.CancellationToken);

                if (profile == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
                }

                return new UserProfileResponse { Profile = profile.ToProto() };
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override async Task<UserResponse> UpdateProfile(UpdateProfileRequest request, ServerCallContext context)
        {
            try
            {
                var userId = Guid.Parse(request.UserId);
                var dto = request.ToDto();
                var user = await _userService.UpdateProfileAsync(userId, dto, context.CancellationToken);

                return new UserResponse { User = user.ToProto() };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override async Task<Empty> ChangePassword(ChangePasswordRequest request, ServerCallContext context)
        {
            try
            {
                var userId = Guid.Parse(request.UserId);
                var dto = request.ToDto();
                await _userService.ChangePasswordAsync(userId, dto, context.CancellationToken);
                return new Empty();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override async Task<Empty> DeleteAccount(DeleteAccountRequest request, ServerCallContext context)
        {
            try
            {
                var userId = Guid.Parse(request.UserId);
                await _userService.DeleteAccountAsync(userId, request.Password, context.CancellationToken);
                return new Empty();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting account");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override async Task<SearchUsersResponse> SearchUsers(SearchUsersRequest request, ServerCallContext context)
        {
            try
            {
                var currentUserId = string.IsNullOrEmpty(request.CurrentUserId)
                    ? (Guid?)null
                    : Guid.Parse(request.CurrentUserId);

                var users = await _userService.SearchUsersAsync(
                    request.SearchTerm,
                    request.PageNumber,
                    request.PageSize,
                    currentUserId,
                    context.CancellationToken);

                var response = new SearchUsersResponse();
                response.Users.AddRange(users.Select(u => u.ToProto()));

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching users");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        #endregion

        #region Follow

        public override async Task<FollowResponse> FollowUser(FollowRequest request, ServerCallContext context)
        {
            try
            {
                var followerId = Guid.Parse(request.FollowerId);
                var followingId = Guid.Parse(request.FollowingId);

                var result = await _followService.FollowUserAsync(followerId, followingId, context.CancellationToken);

                return new FollowResponse
                {
                    IsFollowing = result.IsFollowing,
                    FollowersCount = result.FollowersCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error following user");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override async Task<FollowResponse> UnfollowUser(FollowRequest request, ServerCallContext context)
        {
            try
            {
                var followerId = Guid.Parse(request.FollowerId);
                var followingId = Guid.Parse(request.FollowingId);

                var result = await _followService.UnfollowUserAsync(followerId, followingId, context.CancellationToken);

                return new FollowResponse
                {
                    IsFollowing = result.IsFollowing,
                    FollowersCount = result.FollowersCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unfollowing user");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override async Task<UserListResponse> GetFollowers(GetFollowersRequest request, ServerCallContext context)
        {
            try
            {
                var userId = Guid.Parse(request.UserId);
                var currentUserId = string.IsNullOrEmpty(request.CurrentUserId)
                    ? (Guid?)null
                    : Guid.Parse(request.CurrentUserId);

                var followers = await _followService.GetFollowersAsync(
                    userId,
                    request.PageNumber,
                    request.PageSize,
                    currentUserId,
                    context.CancellationToken);

                var response = new UserListResponse();
                response.Users.AddRange(followers.Select(u => u.ToProto()));

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting followers");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override async Task<UserListResponse> GetFollowing(GetFollowingRequest request, ServerCallContext context)
        {
            try
            {
                var userId = Guid.Parse(request.UserId);
                var currentUserId = string.IsNullOrEmpty(request.CurrentUserId)
                    ? (Guid?)null
                    : Guid.Parse(request.CurrentUserId);

                var following = await _followService.GetFollowingAsync(
                    userId,
                    request.PageNumber,
                    request.PageSize,
                    currentUserId,
                    context.CancellationToken);

                var response = new UserListResponse();
                response.Users.AddRange(following.Select(u => u.ToProto()));

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting following");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override async Task<IsFollowingResponse> IsFollowing(IsFollowingRequest request, ServerCallContext context)
        {
            try
            {
                var followerId = Guid.Parse(request.FollowerId);
                var followingId = Guid.Parse(request.FollowingId);

                var isFollowing = await _followService.IsFollowingAsync(followerId, followingId, context.CancellationToken);

                return new IsFollowingResponse { IsFollowing = isFollowing };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking follow status");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        #endregion

        #region Settings

        public override async Task<UserSettingsResponse> GetUserSettings(GetUserSettingsRequest request, ServerCallContext context)
        {
            try
            {
                var userId = Guid.Parse(request.UserId);
                var settings = await _userSettingsService.GetUserSettingsAsync(userId, context.CancellationToken);

                if (settings == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "User settings not found"));
                }

                return settings.ToProto();
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user settings");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override async Task<UserSettingsResponse> UpdateUserSettings(UpdateUserSettingsRequest request, ServerCallContext context)
        {
            try
            {
                var userId = Guid.Parse(request.UserId);
                var dto = request.ToDto();
                var settings = await _userSettingsService.UpdateUserSettingsAsync(userId, dto, context.CancellationToken);

                return settings.ToProto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user settings");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        #endregion
    }
}
