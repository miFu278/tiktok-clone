namespace TikTok.UserService.Domain.Interfaces
{
    public interface IUnitOfWork : TikTok.Shared.Common.Abstractions.Repositories.IUnitOfWork
    {
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        IFollowRepository Follows { get; }
        IUserSettingsRepository UserSettings { get; }
        IUserStatsRepository UserStats { get; }
        IRefreshTokenRepository RefreshTokens { get; }
    }
}
