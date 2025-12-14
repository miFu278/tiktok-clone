using TikTok.Shared.Common.Abstractions.Repositories;
using TikTok.UserService.Domain.Entities;
using TikTok.UserService.Domain.ValueObjects;

namespace TikTok.UserService.Domain.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        // Authentication & Authorization
        Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
        Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);
        Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default);

        // Email Verification
        Task<User?> GetByEmailVerificationTokenAsync(string token, CancellationToken cancellationToken = default);

        // Password Reset
        Task<User?> GetByPasswordResetTokenAsync(string token, CancellationToken cancellationToken = default);

        // Search
        Task<IEnumerable<User>> SearchUsersAsync(
            string searchTerm,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}
