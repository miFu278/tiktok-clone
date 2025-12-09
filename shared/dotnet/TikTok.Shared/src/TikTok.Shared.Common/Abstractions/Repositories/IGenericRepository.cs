using System.Linq.Expressions;
using TikTok.Shared.Common.Abstractions.Entities;
using TikTok.Shared.Common.Models;

namespace TikTok.Shared.Common.Abstractions.Repositories
{
    public interface IGenericRepository<T> : IRepository<T> where T : BaseEntity
    {
        Task<T?> FirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<bool> AnyAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<int> CountAsync(
            Expression<Func<T, bool>>? predicate = null,
            CancellationToken cancellationToken = default);

        Task<PagedResult<T>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? predicate,
            CancellationToken cancellationToken = default);
    }
}
