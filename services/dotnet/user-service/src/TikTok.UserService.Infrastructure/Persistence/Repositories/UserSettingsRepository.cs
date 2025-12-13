using Microsoft.EntityFrameworkCore;
using TikTok.UserService.Domain.Entities;
using TikTok.UserService.Domain.Interfaces;

namespace TikTok.UserService.Infrastructure.Persistence.Repositories
{
    public class UserSettingsRepository : GenericRepository<UserSettings>, IUserSettingsRepository
    {
        public UserSettingsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<UserSettings?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
        }
    }
}
