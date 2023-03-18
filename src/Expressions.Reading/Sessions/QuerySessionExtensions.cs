using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.Sessions;

public static class QuerySessionExtensions
{
    public static IQuery<TEntity> Query<TEntity>(
        this IQuerySession session)
        where TEntity : class
    {
        return session.Query(QueryModel.Create<TEntity>());
    }

    public static IQuery<TEntity> Query<TEntity>(
        this IQuerySession session,
        Specification<TEntity> specification)
        where TEntity : class
    {
        return session.Query(QueryModel.Create(specification));
    }
}
