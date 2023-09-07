using System.Linq.Expressions;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Queries.Internal;

namespace Raiqub.Expressions.Sessions;

/// <summary>Provides extensions for <see cref="IDbQuerySession"/> interface.</summary>
public static class DbQuerySessionExtensions
{
    /// <summary>Creates a new query that returns all entities.</summary>
    /// <param name="session">The session to create query from.</param>
    /// <typeparam name="TEntity">The type of entity to query.</typeparam>
    /// <returns>A new query object.</returns>
    public static IDbQuery<TEntity> Query<TEntity>(
        this IDbQuerySession session)
        where TEntity : class
    {
        return session.Query(AllQueryModel<TEntity>.Instance);
    }

    /// <summary>Creates a new query using the criteria specification.</summary>
    /// <param name="session">The session to create query from.</param>
    /// <param name="specification">The specification used for query.</param>
    /// <typeparam name="TEntity">The type of entity to query.</typeparam>
    /// <returns>A new query object.</returns>
    public static IDbQuery<TEntity> Query<TEntity>(
        this IDbQuerySession session,
        Specification<TEntity> specification)
        where TEntity : class
    {
        return session.Query(new SpecificationQueryModel<TEntity>(specification));
    }

    public static IDbQuery<TResult> Query<TEntity, TResult>(
        this IDbQuerySession session,
        IEntityQueryModel<TEntity, TResult> entityQueryModel)
        where TEntity : class
    {
        return session.Query(entityQueryModel.ToQueryModel());
    }

    /// <summary>
    /// Creates a new query for the nested collection of the specified entity.
    /// </summary>
    /// <param name="session">The session to create query from.</param>
    /// <param name="selector">A projection function to retrieve entity collection.</param>
    /// <typeparam name="TEntity">The type of entity to query.</typeparam>
    /// <typeparam name="TNested">The type of nested collection from the specified entity.</typeparam>
    /// <returns>A new query object.</returns>
    public static IDbQuery<TNested> QueryNested<TEntity, TNested>(
        this IDbQuerySession session,
        Expression<Func<TEntity, IEnumerable<TNested>>> selector)
        where TEntity : class
        where TNested : class
    {
        return session.Query(
            new NestingEntityQueryModel<TEntity, TNested, TNested>(selector, EntityQueryModel.Create<TNested>()));
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
    public static IDbQuery<TResult> QueryNested<TEntity, TNested, TResult>(
        this IDbQuerySession session,
        Expression<Func<TEntity, IEnumerable<TNested>>> selector,
        IEntityQueryModel<TNested, TResult> queryModel)
        where TEntity : class
        where TNested : class
    {
        return session.Query(new NestingEntityQueryModel<TEntity, TNested, TResult>(selector, queryModel));
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
    public static IDbQuery<TNested> QueryNested<TEntity, TNested>(
        this IDbQuerySession session,
        Expression<Func<TEntity, IEnumerable<TNested>>> selector,
        Specification<TNested> specification)
        where TEntity : class
        where TNested : class
    {
        return session.Query(
            new NestingEntityQueryModel<TEntity, TNested, TNested>(selector, EntityQueryModel.Create(specification)));
    }
}
