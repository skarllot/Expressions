namespace Raiqub.Expressions.Queries.Paging;

internal static class DefaultPagedResultFactory
{
    public static PagedResult<TResult> Create<TResult>(in PageInfo pageInfo, IReadOnlyList<TResult> items)
    {
        return new PagedResult<TResult>(pageInfo, items);
    }
}
