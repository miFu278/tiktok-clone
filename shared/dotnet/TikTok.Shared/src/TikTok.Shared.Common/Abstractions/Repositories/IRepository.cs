using TikTok.Shared.Common.Abstractions.Entities;

namespace TikTok.Shared.Common.Abstractions.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task<T> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
