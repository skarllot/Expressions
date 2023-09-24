using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.Sessions;

/// <summary>Represents a session for querying data from database.</summary>
public interface IDbQuerySession : IAsyncDisposable, IDisposable
{
    /// <summary>Creates a new query using the specified query strategy.</summary>
    /// <typeparam name="TResult">The type of result to return.</typeparam>
    /// <param name="queryStrategy">The query strategy to use.</param>
    /// <returns>A new query object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="queryStrategy"/> is null.</exception>
    IDbQuery<TResult> Query<TResult>(IQueryStrategy<TResult> queryStrategy);
}
