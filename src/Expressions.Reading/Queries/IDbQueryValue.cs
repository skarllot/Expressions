namespace Raiqub.Expressions.Queries;

/// <summary>
/// Represents a query that can be executed to retrieve value type instances of type <typeparamref name="TResult"/>.
/// This interface is specifically designed for value types (structs) and provides nullable return values for optional operations.
/// </summary>
/// <typeparam name="TResult">The value type of the result returned by the query.</typeparam>
public interface IDbQueryValue<TResult> : IDbQueryBase<TResult>
    where TResult : struct
{
    /// <summary>
    /// Returns the first element of the query result, or a default value if the query result contains no elements.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the first element of the query result, or <see langword="null"/> if the query result contains no elements.</returns>
    Task<TResult?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the only element of the query result, or a default value if the query result contains no elements;
    /// this method throws an exception if there is more than one element in the sequence.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the only element of the query result, or <see langword="null"/> if the query result contains no elements.</returns>
    /// <exception cref="InvalidOperationException">Query contains more than one element.</exception>
    /// <exception cref="OperationCanceledException">If the <see cref="CancellationToken" /> is canceled.</exception>
    Task<TResult?> SingleOrDefaultAsync(CancellationToken cancellationToken = default);
}
