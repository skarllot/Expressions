using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.Sessions;

/// <summary>Represents a session for querying data from database.</summary>
public interface IDbQuerySession : IAsyncDisposable, IDisposable
{
    /// <summary>Creates a new query using the specified query model.</summary>
    /// <typeparam name="TEntity">The type of entity to query.</typeparam>
    /// <typeparam name="TResult">The type of result to return.</typeparam>
    /// <param name="queryModel">The query model to use.</param>
    /// <returns>A new query object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="queryModel"/> is null.</exception>
    IQuery<TResult> Query<TEntity, TResult>(IQueryModel<TEntity, TResult> queryModel)
        where TEntity : class;

    /// <summary>Creates a new query using the specified query model.</summary>
    /// <typeparam name="TResult">The type of result to return.</typeparam>
    /// <param name="queryModel">The query model to use.</param>
    /// <returns>A new query object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="queryModel"/> is null.</exception>
    IQuery<TResult> Query<TResult>(IMultiQueryModel<TResult> queryModel);
}
