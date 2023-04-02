namespace Raiqub.Expressions.Queries;

/// <summary>
/// Represents a query that can be executed to retrieve entities of type <typeparamref name="T"/>.
/// </summary>
public interface IQuery<T>
{
    /// <summary>Determines whether the query returns any elements.</summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a value that indicates whether the query returns any elements.</returns>
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets the number of elements in the query result.</summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of elements in the query result.</returns>
    Task<long> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the first element of the query result, or a default value if the query result contains no elements.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the first element of the query result, or <see langword="null"/> if the query result contains no elements.</returns>
    Task<T?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns a read-only list of the elements in the query result.</summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of the elements in the query result.</returns>
    Task<IReadOnlyList<T>> ToListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the only element of the query result, or a default value if the query result contains no elements;
    /// this method throws an exception if there is more than one element in the sequence.</summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the only element of the query result, or <see langword="null"/> if the query result contains no elements or more than one element.</returns>
    /// <exception cref="InvalidOperationException">Query contains more than one element.</exception>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
    Task<T?> SingleOrDefaultAsync(CancellationToken cancellationToken = default);
}
