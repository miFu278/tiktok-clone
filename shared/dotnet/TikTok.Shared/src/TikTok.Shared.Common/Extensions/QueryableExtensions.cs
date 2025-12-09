using Microsoft.EntityFrameworkCore;
using TikTok.Shared.Common.Models;

namespace TikTok.Shared.Common.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
            this IQueryable<T> query,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;

            var totalCount = await query.CountAsync(cancellationToken);
            
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<T>(items, totalCount, pageNumber, pageSize);
        }

        public static IQueryable<T> WhereIf<T>(
            this IQueryable<T> query,
            bool condition,
            System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            return condition ? query.Where(predicate) : query;
        }

        public static IQueryable<T> OrderByIf<T, TKey>(
            this IQueryable<T> query,
            bool condition,
            System.Linq.Expressions.Expression<Func<T, TKey>> keySelector)
        {
            return condition ? query.OrderBy(keySelector) : query;
        }

        public static IQueryable<T> OrderByDescendingIf<T, TKey>(
            this IQueryable<T> query,
            bool condition,
            System.Linq.Expressions.Expression<Func<T, TKey>> keySelector)
        {
            return condition ? query.OrderByDescending(keySelector) : query;
        }
    }
}
