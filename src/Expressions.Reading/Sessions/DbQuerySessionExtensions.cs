using System.Linq.Expressions;
using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.Sessions;

/// <summary>Provides extensions for <see cref="IDbQuerySession"/> interface.</summary>
public static class DbQuerySessionExtensions
{
    /// <summary>Creates a new query that returns all entities.</summary>
    /// <param name="session">The session to create query from.</param>
    /// <typeparam name="TEntity">The type of entity to query.</typeparam>
    /// <returns>A new query object.</returns>
    public static IQuery<TEntity> Query<TEntity>(
        this IDbQuerySession session)
        where TEntity : class
    {
        return session.Query(QueryModel.Create<TEntity>());
    }

    /// <summary>Creates a new query using the criteria specification.</summary>
    /// <param name="session">The session to create query from.</param>
    /// <param name="specification">The specification used for query.</param>
    /// <typeparam name="TEntity">The type of entity to query.</typeparam>
    /// <returns>A new query object.</returns>
    public static IQuery<TEntity> Query<TEntity>(
        this IDbQuerySession session,
        Specification<TEntity> specification)
        where TEntity : class
    {
        return session.Query(QueryModel.Create(specification));
    }

    /// <summary>
    /// Creates a new query for the nested collection of the specified entity.
    /// </summary>
    /// <param name="session">The session to create query from.</param>
    /// <param name="selector">A projection function to retrieve entity collection.</param>
    /// <typeparam name="TEntity">The type of entity to query.</typeparam>
    /// <typeparam name="TNested">The type of nested collection from the specified entity.</typeparam>
    /// <returns>A new query object.</returns>
    public static IQuery<TNested> QueryNested<TEntity, TNested>(
        this IDbQuerySession session,
        Expression<Func<TEntity, IEnumerable<TNested>>> selector)
        where TEntity : class
    {
        return session.Query(new NestingQueryModel<TEntity, TNested, TNested>(selector, QueryModel.Create<TNested>()));
    }

    /// <summary>
    /// Creates a new query for the nested collection of the specified entity using the specified query model.
    /// </summary>
    /// <param name="session">The session to create query from.</param>
    /// <param name="selector">A projection function to retrieve entity collection.</param>
    /// <param name="queryModel">The query model to use.</param>
    /// <typeparam name="TEntity">The type of entity to query.</typeparam>
    /// <typeparam name="TNested">The type of nested collection from the specified entity.</typeparam>
    /// <typeparam name="TResult">The type of result to return.</typeparam>
    /// <returns>A new query object.</returns>
    public static IQuery<TResult> QueryNested<TEntity, TNested, TResult>(
        this IDbQuerySession session,
        Expression<Func<TEntity, IEnumerable<TNested>>> selector,
        IQueryModel<TNested, TResult> queryModel)
        where TEntity : class
    {
        return session.Query(new NestingQueryModel<TEntity, TNested, TResult>(selector, queryModel));
    }

    /// <summary>
    /// Creates a new query for the nested collection of the specified entity using the criteria specification.
    /// </summary>
    /// <param name="session">The session to create query from.</param>
    /// <param name="selector">A projection function to retrieve entity collection.</param>
    /// <param name="specification">The specification used for query.</param>
    /// <typeparam name="TEntity">The type of entity to query.</typeparam>
    /// <typeparam name="TNested">The type of nested collection from the specified entity.</typeparam>
    /// <returns>A new query object.</returns>
    public static IQuery<TNested> QueryNested<TEntity, TNested>(
        this IDbQuerySession session,
        Expression<Func<TEntity, IEnumerable<TNested>>> selector,
        Specification<TNested> specification)
        where TEntity : class
    {
        return session.Query(
            new NestingQueryModel<TEntity, TNested, TNested>(selector, QueryModel.Create(specification)));
    }
}
