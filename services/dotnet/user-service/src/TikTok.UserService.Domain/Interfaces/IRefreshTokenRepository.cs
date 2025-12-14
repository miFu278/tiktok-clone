using TikTok.Shared.Common.Abstractions.Repositories;
using TikTok.UserService.Domain.Entities;

namespace TikTok.UserService.Domain.Interfaces
{
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
        Task<IEnumerable<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task RevokeTokenAsync(string token, string? revokedByIp = null, string? reason = null, string? replacedByToken = null, CancellationToken cancellationToken = default);
        Task RevokeAllUserTokensAsync(Guid userId, string? reason = null, CancellationToken cancellationToken = default);
        Task RemoveExpiredTokensAsync(CancellationToken cancellationToken = default);
    }
}
