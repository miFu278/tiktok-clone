using TikTok.UserService.Domain.Interfaces;
using TikTok.UserService.Infrastructure.Persistence.Repositories;

namespace TikTok.UserService.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IUserRepository? _users;
        private IRoleRepository? _roles;
        private IFollowRepository? _follows;
        private IUserSettingsRepository? _userSettings;
        private IUserStatsRepository? _userStats;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        private IRefreshTokenRepository? _refreshTokens;

        public IUserRepository Users => _users ??= new UserRepository(_context);
        public IRoleRepository Roles => _roles ??= new RoleRepository(_context);
        public IFollowRepository Follows => _follows ??= new FollowRepository(_context);
        public IUserSettingsRepository UserSettings => _userSettings ??= new UserSettingsRepository(_context);
        public IUserStatsRepository UserStats => _userStats ??= new UserStatsRepository(_context);
        public IRefreshTokenRepository RefreshTokens => _refreshTokens ??= new RefreshTokenRepository(_context);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
