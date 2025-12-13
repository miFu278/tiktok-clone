using TikTok.UserService.Application.DTOs;

namespace TikTok.UserService.Application.Interfaces
{
    public interface IAuthService
    {
        Task<UserDto> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default);
        Task<LoginResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
        Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenDto dto, CancellationToken cancellationToken = default);
        Task<bool> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task<bool> LogoutAllDevicesAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<bool> VerifyEmailAsync(VerifyEmailDto dto, CancellationToken cancellationToken = default);
        Task<bool> ResendEmailVerificationAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> ForgotPasswordAsync(ForgotPasswordDto dto, CancellationToken cancellationToken = default);
        Task<bool> ResetPasswordAsync(ResetPasswordDto dto, CancellationToken cancellationToken = default);
    }
}
