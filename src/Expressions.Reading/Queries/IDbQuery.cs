using Raiqub.Expressions.Queries.Paging;

namespace Raiqub.Expressions.Queries;

/// <summary>
/// Represents a query that can be executed to retrieve entities of type <typeparamref name="T"/>.
/// </summary>
public interface IDbQuery<T>
{
    /// <summary>Determines whether the query returns any elements.</summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a value that indicates whether the query returns any elements.</returns>
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets the number of elements in the query result.</summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of elements in the query result.</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the first element of the query result.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the first element of the query result.</returns>
    /// <exception cref="InvalidOperationException">Query result contains no elements.</exception>
    Task<T> FirstAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the first element of the query result, or a default value if the query result contains no elements.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the first element of the query result, or <see langword="null"/> if the query result contains no elements.</returns>
    Task<T?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns a page from the available elements in the query result.</summary>
    /// <param name="pageNumber">The one-based page number to retrieve.</param>
    /// <param name="pageSize">The maximum number of elements to return.</param>
    /// <param name="pagedResultFactory">The factory to build the paged result.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the elements of the page and related information to help pagination.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="pageNumber"/> or <paramref name="pageSize"/> is less than 1.</exception>
    Task<TPage> ToPagedListAsync<TPage>(
        int pageNumber,
        int pageSize,
        PagedResultFactory<T, TPage> pagedResultFactory,
        CancellationToken cancellationToken = default);

    /// <summary>Returns a read-only list of the elements in the query result.</summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of the elements in the query result.</returns>
    Task<IReadOnlyList<T>> ToListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the only element of the query result,
    /// and throws an exception if there is not exactly one element in the sequence.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the only element of the query result.</returns>
    /// <exception cref="InvalidOperationException">Query contains more than one element or contains no elements.</exception>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
    Task<T> SingleAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the only element of the query result, or a default value if the query result contains no elements;
    /// this method throws an exception if there is more than one element in the sequence.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the only element of the query result, or <see langword="null"/> if the query result contains no elements.</returns>
    /// <exception cref="InvalidOperationException">Query contains more than one element.</exception>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
    Task<T?> SingleOrDefaultAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Execute this query to an <see cref="IAsyncEnumerable{T}"/>. This is valuable for reading
    /// and processing large result sets without having to keep the entire result set in memory
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>The query results.</returns>
    IAsyncEnumerable<T> ToAsyncEnumerable(CancellationToken cancellationToken = default);
}
