using Raiqub.Expressions.Queries.Paging;

namespace Raiqub.Expressions.Queries;

/// <summary>Provides extensions for <see cref="IDbQuery{T}"/> interface.</summary>
public static class DbQueryExtensions
{
    /// <summary>Returns a page from the available elements in the query result.</summary>
    /// <param name="dbQuery">The database query.</param>
    /// <param name="pageNumber">The one-based page number to retrieve.</param>
    /// <param name="pageSize">The maximum number of elements to return.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the elements of the page and related information to help pagination.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="pageNumber"/> or <paramref name="pageSize"/> is less than 1.</exception>
    public static Task<PagedResult<TResult>> ToPagedListAsync<TResult>(
        this IDbQuery<TResult> dbQuery,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return dbQuery.ToPagedListAsync(pageNumber, pageSize, DefaultPagedResultFactory.Create, cancellationToken);
    }
}
