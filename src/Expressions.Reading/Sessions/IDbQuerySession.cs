using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.Sessions;

/// <summary>Represents a session for querying data from database.</summary>
public interface IDbQuerySession : IAsyncDisposable, IDisposable
{
    /// <summary>Creates a new query that returns all entities.</summary>
    /// <typeparam name="TEntity">The type of entity to query.</typeparam>
    /// <returns>A new query object.</returns>
    IDbQuery<TEntity> Query<TEntity>() where TEntity : class;

    /// <summary>Creates a new query using the specification to test a business rule.</summary>
    /// <param name="specification">The specification used for query.</param>
    /// <typeparam name="TEntity">The type of entity to query.</typeparam>
    /// <returns>A new query object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="specification"/> is null.</exception>
    IDbQuery<TEntity> Query<TEntity>(Specification<TEntity> specification) where TEntity : class;

    /// <summary>Creates a new query using the specified query strategy.</summary>
    /// <typeparam name="TEntity">The type of entity to query.</typeparam>
    /// <typeparam name="TResult">The type of result to return.</typeparam>
    /// <param name="queryStrategy">The query strategy to use.</param>
    /// <returns>A new query object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="queryStrategy"/> is null.</exception>
    IDbQuery<TResult> Query<TEntity, TResult>(IEntityQueryStrategy<TEntity, TResult> queryStrategy)
        where TEntity : class
        where TResult : notnull;

    /// <summary>Creates a new query using the specified query strategy.</summary>
    /// <typeparam name="TResult">The type of result to return.</typeparam>
    /// <param name="queryStrategy">The query strategy to use.</param>
    /// <returns>A new query object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="queryStrategy"/> is null.</exception>
    IDbQuery<TResult> Query<TResult>(IQueryStrategy<TResult> queryStrategy)
        where TResult : notnull;

    /// <summary>Creates a new query using the specified entity query strategy for value types.</summary>
    /// <typeparam name="TEntity">The type of entity to query.</typeparam>
    /// <typeparam name="TResult">The value type of result to return.</typeparam>
    /// <param name="queryStrategy">The entity query strategy to use.</param>
    /// <returns>A new query object for value types.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="queryStrategy"/> is null.</exception>
    IDbQueryValue<TResult> QueryValue<TEntity, TResult>(IEntityQueryStrategy<TEntity, TResult> queryStrategy)
        where TEntity : class
        where TResult : struct;

    /// <summary>Creates a new query using the specified query strategy for value types.</summary>
    /// <typeparam name="TResult">The value type of result to return.</typeparam>
    /// <param name="queryStrategy">The query strategy to use.</param>
    /// <returns>A new query object for value types.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="queryStrategy"/> is null.</exception>
    IDbQueryValue<TResult> QueryValue<TResult>(IQueryStrategy<TResult> queryStrategy)
        where TResult : struct;
}
