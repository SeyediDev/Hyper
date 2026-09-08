using System.Linq.Expressions;

namespace Hyper.Domain.Features.Promotions;

/// <summary>
/// Extension methods for IQueryable to support async operations
/// </summary>
internal static class QueryableExtensions
{
    public static async Task<long> SumAsync<T>(this IQueryable<T> query, Expression<Func<T, long>> selector, CancellationToken cancellationToken)
    {
        return await Task.Run(() => query.Sum(selector), cancellationToken);
    }

    public static async Task<int> CountAsync<T>(this IQueryable<T> query, Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        return await Task.Run(() => query.Count(predicate), cancellationToken);
    }
}
