using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Expressions.Repositories;

public static class RepositoryExtensions
{
    public static IQuery<TEntity> Query<TContext, TEntity>(
        this IReadRepository<TContext, TEntity> repository,
        IReadSession<TContext>? session = null)
    {
        return repository.Query(QueryModel.Create<TEntity>(), session);
    }

    public static IQuery<TEntity> Query<TContext, TEntity>(
        this IReadRepository<TContext, TEntity> repository,
        Specification<TEntity> specification,
        IReadSession<TContext>? session = null)
    {
        return repository.Query(QueryModel.Create(specification), session);
    }
}
