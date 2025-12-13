using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TikTok.Shared.Security.Interfaces;
using TikTok.Shared.Security.Models;

namespace TikTok.Shared.Security.Services
{
    public class JwtTokenService : ITokenService
    {
        private readonly TokenOptions _options;
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public JwtTokenService(IOptions<TokenOptions> options)
        {
            _options = options.Value;
            _tokenHandler = new JwtSecurityTokenHandler();
        }

        public string GenerateAccessToken(TokenPayload payload)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, payload.UserId),
                new(ClaimTypes.Email, payload.Email),
                new(ClaimTypes.Name, payload.Username),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Add roles
            claims.AddRange(payload.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Add custom claims
            claims.AddRange(payload.Claims.Select(kvp => new Claim(kvp.Key, kvp.Value)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_options.AccessTokenExpirationMinutes),
                signingCredentials: credentials
            );

            return _tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _options.Issuer,
                    ValidAudience = _options.Audience,
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = _tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        public string? GetUserIdFromToken(string token)
        {
            var principal = ValidateToken(token);
            return principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public bool IsTokenExpired(string token)
        {
            try
            {
                var jwtToken = _tokenHandler.ReadJwtToken(token);
                return jwtToken.ValidTo < DateTime.UtcNow;
            }
            catch
            {
                return true;
            }
        }
    }
}
