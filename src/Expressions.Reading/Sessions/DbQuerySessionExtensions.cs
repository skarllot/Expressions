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
        return session.Query(AllQueryStrategy<TEntity>.Instance);
    }

    /// <summary>Creates a new query using the specification to test a business rule.</summary>
    /// <param name="session">The session to create query from.</param>
    /// <param name="specification">The specification used for query.</param>
    /// <typeparam name="TEntity">The type of entity to query.</typeparam>
    /// <returns>A new query object.</returns>
    public static IDbQuery<TEntity> Query<TEntity>(
        this IDbQuerySession session,
        Specification<TEntity> specification)
        where TEntity : class
    {
        return session.Query(new SpecificationQueryStrategy<TEntity>(specification));
    }
}
