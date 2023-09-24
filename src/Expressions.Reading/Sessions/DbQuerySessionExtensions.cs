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

    /// <summary>Creates a new query using the specified business rule.</summary>
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

    /// <summary>Creates a new query using the specified entity query model.</summary>
    /// <param name="session">The session to create query from.</param>
    /// <typeparam name="TEntity">The type of entity to query.</typeparam>
    /// <typeparam name="TResult">The type of result to return.</typeparam>
    /// <param name="entityQueryModel">The query model to use.</param>
    /// <returns>A new query object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="entityQueryModel"/> is null.</exception>
    public static IDbQuery<TResult> Query<TEntity, TResult>(
        this IDbQuerySession session,
        IEntityQueryModel<TEntity, TResult> entityQueryModel)
        where TEntity : class
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(entityQueryModel);
#else
        if (entityQueryModel is null) throw new ArgumentNullException(nameof(entityQueryModel));
#endif

        return session.Query(entityQueryModel.ToQueryModel());
    }

    [Obsolete("Use the CreateNested method from QueryModel static class", true)]
    public static IDbQuery<TNested> QueryNested<TEntity, TNested>(
        this IDbQuerySession session,
        Expression<Func<TEntity, IEnumerable<TNested>>> selector)
        where TEntity : class
        where TNested : class
    {
        return session.Query(
            new NestingEntityQueryModel<TEntity, TNested, TNested>(selector, QueryModel.AllOfEntity<TNested>()));
    }

    [Obsolete("Use the CreateNested method from QueryModel static class", true)]
    public static IDbQuery<TResult> QueryNested<TEntity, TNested, TResult>(
        this IDbQuerySession session,
        Expression<Func<TEntity, IEnumerable<TNested>>> selector,
        IEntityQueryModel<TNested, TResult> queryModel)
        where TEntity : class
        where TNested : class
    {
        return session.Query(new NestingEntityQueryModel<TEntity, TNested, TResult>(selector, queryModel));
    }

    [Obsolete("Use the CreateNested method from QueryModel static class", true)]
    public static IDbQuery<TNested> QueryNested<TEntity, TNested>(
        this IDbQuerySession session,
        Expression<Func<TEntity, IEnumerable<TNested>>> selector,
        Specification<TNested> specification)
        where TEntity : class
        where TNested : class
    {
        return session.Query(
            new NestingEntityQueryModel<TEntity, TNested, TNested>(
                selector,
                QueryModel.CreateForEntity(specification)));
    }
}
