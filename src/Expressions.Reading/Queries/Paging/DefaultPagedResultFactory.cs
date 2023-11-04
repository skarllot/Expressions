namespace Raiqub.Expressions.Queries.Paging;

/// <summary>
/// Provides <see cref="PagedResultFactory{TResult,TPage}"/> for <see cref="PagedResult{TResult}"/>.
/// </summary>
/// <typeparam name="TResult">The type of elements returned by the query.</typeparam>
public static class DefaultPagedResultFactory<TResult>
{
    /// <summary>Represents a factory to build paged results of <see cref="PagedResult{TResult}"/>.</summary>
    public static readonly PagedResultFactory<TResult, PagedResult<TResult>> Create =
        (in PageInfo info, IReadOnlyList<TResult> items) => new PagedResult<TResult>(info, items);
}
