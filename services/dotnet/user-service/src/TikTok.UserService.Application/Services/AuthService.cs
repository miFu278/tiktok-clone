using AutoMapper;
using TikTok.Shared.Common.Exceptions;
using TikTok.Shared.Common.Interfaces;
using TikTok.Shared.Security.Interfaces;
using TikTok.Shared.Security.Models;
using TikTok.UserService.Application.DTOs;
using TikTok.UserService.Application.Interfaces;
using TikTok.UserService.Domain.Entities;
using TikTok.UserService.Domain.Exceptions;
using TikTok.UserService.Domain.Interfaces;
using TikTok.UserService.Domain.ValueObjects;

namespace TikTok.UserService.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;

        public AuthService(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            ITokenGenerator tokenGenerator,
            IEmailService emailService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _tokenGenerator = tokenGenerator;
            _emailService = emailService;
            _mapper = mapper;
        }

        public async Task<UserDto> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default)
        {
            // Validate email
            var email = Email.Create(dto.Email);

            // Check if email already exists
            if (await _unitOfWork.Users.ExistsByEmailAsync(email, cancellationToken))
            {
                throw new DuplicateEmailException(dto.Email);
            }

            // Check if username already exists (if provided)
            if (!string.IsNullOrEmpty(dto.UserName))
            {
                if (await _unitOfWork.Users.ExistsByUsernameAsync(dto.UserName, cancellationToken))
                {
                    throw new DuplicateUsernameException(dto.UserName);
                }
            }

            // Create user entity
            var user = _mapper.Map<User>(dto);
            user.Email = email.Value;
            user.PasswordHash = _passwordHasher.HashPassword(dto.Password);
            user.EmailVerified = false;
            user.IsActive = true;
            user.EmailVerificationToken = _tokenGenerator.GenerateVerificationToken();
            user.EmailVerificationTokenExpires = DateTime.UtcNow.AddHours(24);

            // Add user
            await _unitOfWork.Users.AddAsync(user, cancellationToken);

            // Assign default "User" role
            var userRole = await _unitOfWork.Roles.GetByNameAsync("User", cancellationToken);
            if (userRole != null)
            {
                var userRoleEntity = new UserRole
                {
                    UserId = user.Id,
                    RoleId = userRole.Id,
                    AssignedAt = DateTime.UtcNow
                };
                user.UserRoles.Add(userRoleEntity);
            }

            // Create default settings
            var settings = new UserSettings
            {
                UserId = user.Id,
                IsPrivateAccount = false,
                AllowComments = true,
                AllowDuet = true,
                AllowStitch = true,
                AllowDownload = true,
                PushNotificationsEnabled = true,
                EmailNotificationsEnabled = true,
                NotifyOnLikes = true,
                NotifyOnComments = true,
                NotifyOnFollows = true,
                NotifyOnMentions = true
            };
            await _unitOfWork.UserSettings.AddAsync(settings, cancellationToken);

            // Create default stats
            var stats = new UserStats
            {
                UserId = user.Id
            };
            await _unitOfWork.UserStats.AddAsync(stats, cancellationToken);

            // Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Send verification email
            await _emailService.SendEmailVerificationAsync(
                user.Email,
                user.FullName ?? user.UserName ?? "User",
                user.EmailVerificationToken,
                cancellationToken);

            return _mapper.Map<UserDto>(user);
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
        {
            // Find user by email
            var email = Email.Create(dto.Email);
            var user = await _unitOfWork.Users.GetByEmailAsync(email, cancellationToken);

            if (user == null)
            {
                throw new UnauthorizedException("Invalid email or password");
            }

            // Verify password
            if (!_passwordHasher.VerifyPassword(dto.Password, user.PasswordHash ?? string.Empty))
            {
                user.IncrementFailedLoginAttempts();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                throw new UnauthorizedException("Invalid email or password");
            }

            // Check if account is locked
            if (user.IsLockedOut)
            {
                throw new ForbiddenException($"Account is locked until {user.LockoutEnd}");
            }

            // Check if account is active
            if (!user.IsActive)
            {
                throw new ForbiddenException("Account is deactivated");
            }

            // Check if email is verified
            // if (!user.EmailVerified)
            // {
            //     throw new ForbiddenException("Please verify your email before logging in");
            // }

            // Reset failed login attempts
            user.ResetFailedLoginAttempts();

            // Generate tokens
            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            var tokenPayload = new TokenPayload
            {
                UserId = user.Id.ToString(),
                Email = user.Email,
                Username = user.UserName ?? user.Email,
                Roles = roles
            };

            var accessToken = _tokenService.GenerateAccessToken(tokenPayload);
            var refreshToken = _tokenGenerator.GenerateRefreshToken();

            // Save refresh token to database
            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60), // Should match JWT expiration
                User = _mapper.Map<UserDto>(user)
            };
        }

        public async Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken cancellationToken = default)
        {
            // Get refresh token from database
            var refreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(dto.RefreshToken, cancellationToken);

            if (refreshToken == null)
            {
                throw new UnauthorizedException("Invalid refresh token");
            }

            // Check if token is active
            if (!refreshToken.IsActive)
            {
                // Token has been revoked or expired
                // Revoke all tokens for this user (possible token theft)
                await _unitOfWork.RefreshTokens.RevokeAllUserTokensAsync(
                    refreshToken.UserId,
                    "Attempted reuse of revoked token",
                    cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                throw new UnauthorizedException("Invalid refresh token");
            }

            // Get user
            var user = await _unitOfWork.Users.GetByIdAsync(refreshToken.UserId, cancellationToken);
            if (user == null)
            {
                throw new UserNotFoundException(refreshToken.UserId);
            }

            // Check if user is active
            if (!user.IsActive)
            {
                throw new ForbiddenException("Account is deactivated");
            }

            // Generate new tokens
            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            var tokenPayload = new TokenPayload
            {
                UserId = user.Id.ToString(),
                Email = user.Email,
                Username = user.UserName ?? user.Email,
                Roles = roles
            };

            var newAccessToken = _tokenService.GenerateAccessToken(tokenPayload);
            var newRefreshToken = _tokenGenerator.GenerateRefreshToken();

            // Revoke old refresh token and save new one (token rotation)
            await _unitOfWork.RefreshTokens.RevokeTokenAsync(
                refreshToken.Token,
                reason: "Replaced by new token",
                replacedByToken: newRefreshToken,
                cancellationToken: cancellationToken);

            var newRefreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.RefreshTokens.AddAsync(newRefreshTokenEntity, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new LoginResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                User = _mapper.Map<UserDto>(user)
            };
        }

        public async Task<bool> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                return false;
            }

            // Revoke the refresh token
            await _unitOfWork.RefreshTokens.RevokeTokenAsync(
                refreshToken,
                reason: "Logged out",
                cancellationToken: cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> LogoutAllDevicesAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            // Revoke all active refresh tokens for the user
            await _unitOfWork.RefreshTokens.RevokeAllUserTokensAsync(
                userId,
                "Logged out from all devices",
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> VerifyEmailAsync(VerifyEmailDto dto, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByEmailVerificationTokenAsync(dto.Token, cancellationToken);

            if (user == null)
            {
                throw new NotFoundException("Invalid verification token");
            }

            if (!user.IsEmailVerificationTokenValid)
            {
                throw new BadRequestException("Verification token has expired");
            }

            user.EmailVerified = true;
            user.EmailVerificationToken = null;
            user.EmailVerificationTokenExpires = null;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Send welcome email
            await _emailService.SendWelcomeEmailAsync(
                user.Email,
                user.FullName ?? user.UserName ?? "User",
                cancellationToken);

            return true;
        }

        public async Task<bool> ResendEmailVerificationAsync(string email, CancellationToken cancellationToken = default)
        {
            var emailVO = Email.Create(email);
            var user = await _unitOfWork.Users.GetByEmailAsync(emailVO, cancellationToken);

            if (user == null)
            {
                throw new UserNotFoundException(email);
            }

            if (user.EmailVerified)
            {
                throw new BadRequestException("Email is already verified");
            }

            // Generate new token
            user.EmailVerificationToken = _tokenGenerator.GenerateVerificationToken();
            user.EmailVerificationTokenExpires = DateTime.UtcNow.AddHours(24);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Send verification email
            await _emailService.SendEmailVerificationAsync(
                user.Email,
                user.FullName ?? user.UserName ?? "User",
                user.EmailVerificationToken,
                cancellationToken);

            return true;
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto dto, CancellationToken cancellationToken = default)
        {
            var email = Email.Create(dto.Email);
            var user = await _unitOfWork.Users.GetByEmailAsync(email, cancellationToken);

            if (user == null)
            {
                // Don't reveal that user doesn't exist
                return true;
            }

            // Generate reset token
            user.PasswordResetToken = _tokenGenerator.GenerateResetPasswordToken();
            user.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(1);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Send reset email
            await _emailService.SendPasswordResetAsync(
                user.Email,
                user.FullName ?? user.UserName ?? "User",
                user.PasswordResetToken,
                cancellationToken);

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByPasswordResetTokenAsync(dto.Token, cancellationToken);

            if (user == null)
            {
                throw new NotFoundException("Invalid reset token");
            }

            if (!user.IsPasswordResetTokenValid)
            {
                throw new BadRequestException("Reset token has expired");
            }

            // Hash new password
            user.PasswordHash = _passwordHasher.HashPassword(dto.NewPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpires = null;

            // Unlock account if it was locked
            if (user.IsLocked)
            {
                user.UnlockAccount();
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
