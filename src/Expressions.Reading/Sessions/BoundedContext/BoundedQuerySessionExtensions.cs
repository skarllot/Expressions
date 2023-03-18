using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.Sessions.BoundedContext;

public static class BoundedQuerySessionExtensions
{
    public static IQuery<TEntity> Query<TContext, TEntity>(
        this IQuerySession<TContext> session)
        where TEntity : class
    {
        return session.Query(QueryModel.Create<TEntity>());
    }

    public static IQuery<TEntity> Query<TContext, TEntity>(
        this IQuerySession<TContext> session,
        Specification<TEntity> specification)
        where TEntity : class
    {
        return session.Query(QueryModel.Create(specification));
    }
}
