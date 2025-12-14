using TikTok.Shared.Common.Abstractions.Repositories;
using TikTok.UserService.Domain.Entities;

namespace TikTok.UserService.Domain.Interfaces
{
    public interface IUserSettingsRepository : IGenericRepository<UserSettings>
    {
        Task<UserSettings?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
