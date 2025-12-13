using Microsoft.EntityFrameworkCore;
using TikTok.UserService.Domain.Entities;
using TikTok.UserService.Domain.Interfaces;

namespace TikTok.UserService.Infrastructure.Persistence.Repositories
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
        }

        public async Task<IEnumerable<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(rt => rt.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task RevokeTokenAsync(
            string token,
            string? revokedByIp = null,
            string? reason = null,
            string? replacedByToken = null,
            CancellationToken cancellationToken = default)
        {
            var refreshToken = await GetByTokenAsync(token, cancellationToken);
            if (refreshToken != null && refreshToken.IsActive)
            {
                refreshToken.RevokedAt = DateTime.UtcNow;
                refreshToken.RevokedByIp = revokedByIp;
                refreshToken.ReasonRevoked = reason;
                refreshToken.ReplacedByToken = replacedByToken;
                _dbSet.Update(refreshToken);
            }
        }

        public async Task RevokeAllUserTokensAsync(Guid userId, string? reason = null, CancellationToken cancellationToken = default)
        {
            var tokens = await GetActiveTokensByUserIdAsync(userId, cancellationToken);
            foreach (var token in tokens)
            {
                token.RevokedAt = DateTime.UtcNow;
                token.ReasonRevoked = reason ?? "All tokens revoked";
            }
            _dbSet.UpdateRange(tokens);
        }

        public async Task RemoveExpiredTokensAsync(CancellationToken cancellationToken = default)
        {
            var expiredTokens = await _dbSet
                .Where(rt => rt.ExpiresAt < DateTime.UtcNow.AddDays(-30)) // Keep for 30 days after expiry
                .ToListAsync(cancellationToken);

            _dbSet.RemoveRange(expiredTokens);
        }
    }
}
