using Neo.Infrastructure.Data.Repository.Ef;
using Neo.Domain.Repository;
using Hyper.Domain.Repository;

namespace Hyper.Infrastructure.Data.Repository.Hyper;

public class CommandHyperEntityRepository<TEntity>(IHyperUnitOfWorkCommand commandUnitOfWork)
	: CommandHyperEntityRepository<TEntity, int>(commandUnitOfWork), ICommandRepository<TEntity>
	where TEntity : class, IEntity<int>, new()
{
}

public class CommandHyperEntityRepositoryL<TEntity>(IHyperUnitOfWorkCommand commandUnitOfWork)
    : CommandHyperEntityRepository<TEntity, long>(commandUnitOfWork), ICommandRepositoryL<TEntity>
    where TEntity : class, IEntity<long>, new()
{
}

public class CommandHyperEntityRepository<TEntity, TKey>(IHyperUnitOfWorkCommand commandUnitOfWork)
    : EfCommandRepository<TEntity, TKey, IHyperUnitOfWorkCommand>(commandUnitOfWork), ICommandRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>, new()
    where TKey : struct
{
}

public class QueryHyperEntityRepository<TEntity>(IHyperUnitOfWorkQuery queryUnitOfWork)
	: QueryHyperEntityRepository<TEntity, int>(queryUnitOfWork), IQueryRepository<TEntity>
	where TEntity : class, IEntity<int>, new()
{
}

public class QueryHyperEntityRepositoryL<TEntity>(IHyperUnitOfWorkQuery queryUnitOfWork)
    : QueryHyperEntityRepository<TEntity, long>(queryUnitOfWork), IQueryRepositoryL<TEntity>
    where TEntity : class, IEntity<long>, new()
{
}

public class QueryHyperEntityRepository<TEntity, TKey>(IHyperUnitOfWorkQuery queryUnitOfWork)
    : EfQueryRepository<TEntity, TKey, IHyperUnitOfWorkQuery>(queryUnitOfWork), IQueryRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>, new()
    where TKey : struct
{
}