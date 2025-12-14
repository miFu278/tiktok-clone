using System.Security.Claims;
using TikTok.Shared.Security.Models;

namespace TikTok.Shared.Security.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(TokenPayload payload);
        string GenerateRefreshToken();
        ClaimsPrincipal? ValidateToken(string token);
        string? GetUserIdFromToken(string token);
        bool IsTokenExpired(string token);
    }
}
