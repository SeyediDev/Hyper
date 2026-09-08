using Neo.Domain.Repository;
using Hyper.Domain.Repository;

namespace Hyper.Infrastructure.Data.Repository.Hyper;

/// <summary>
/// Extension methods for IHyperUnitOfWorkCommand and IHyperUnitOfWorkQuery to access repositories
/// </summary>
public static class HyperUnitOfWorkExtensions
{
    /// <summary>
    /// Gets a command repository for the specified entity type
    /// </summary>
    public static ICommandRepository<TEntity, TKey> Repository<TEntity, TKey>(
        this IHyperUnitOfWorkCommand unitOfWork)
        where TEntity : class, IEntity<TKey>, new()
        where TKey : struct
    {
        return new CommandHyperEntityRepository<TEntity, TKey>(unitOfWork);
    }

    /// <summary>
    /// Gets a query repository for the specified entity type
    /// </summary>
    public static IQueryRepository<TEntity, TKey> Repository<TEntity, TKey>(
        this IHyperUnitOfWorkQuery unitOfWork)
        where TEntity : class, IEntity<TKey>, new()
        where TKey : struct
    {
        return new QueryHyperEntityRepository<TEntity, TKey>(unitOfWork);
    }
}

