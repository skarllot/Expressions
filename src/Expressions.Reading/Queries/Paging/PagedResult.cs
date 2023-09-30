using System.Collections;

namespace Raiqub.Expressions.Queries.Paging;

/// <summary>Represents the result of paged query.</summary>
/// <typeparam name="TResult">The type of items returned by the query.</typeparam>
public sealed class PagedResult<TResult> : IReadOnlyList<TResult>
{
    private readonly PageInfo _pageInfo;
    private readonly IReadOnlyList<TResult> _items;

    public PagedResult(PageInfo pageInfo, IReadOnlyList<TResult> items)
    {
        _pageInfo = pageInfo;
        _items = items;
    }

    /// <inheritdoc />
    public TResult this[int index] => _items[index];

    /// <inheritdoc />
    public int Count => _items.Count;

    /// <summary>Gets current page number (one-based).</summary>
    public long PageNumber => _pageInfo.PageNumber;

    /// <summary>Gets page size.</summary>
    public int PageSize => _pageInfo.PageSize;

    /// <summary>Gets the total number of records.</summary>
    public long TotalCount => _pageInfo.TotalCount;

    /// <summary>Gets number of available pages.</summary>
    public long PageCount => _pageInfo.PageCount;

    /// <summary>Gets a value indicating whether there is a previous page.</summary>
    public bool HasPreviousPage => _pageInfo.HasPreviousPage;

    /// <summary>Gets a value indicating whether there is a next page.</summary>
    public bool HasNextPage => _pageInfo.HasNextPage;

    /// <summary>Gets a value indicating whether the current page is the first page.</summary>
    public bool IsFirstPage => _pageInfo.IsFirstPage;

    /// <summary>Gets a value indicating whether the current page is the last page.</summary>
    public bool IsLastPage => _pageInfo.IsLastPage;

    /// <summary>Gets one-based index of first item in current page.</summary>
    public long FirstItemOnPage => _pageInfo.FirstItemOnPage;

    /// <summary>Gets one-based index of last item in current page.</summary>
    public long LastItemOnPage => _pageInfo.GetLastItemOnPage(_items.Count);

    /// <inheritdoc />
    public IEnumerator<TResult> GetEnumerator() => _items.GetEnumerator();

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
