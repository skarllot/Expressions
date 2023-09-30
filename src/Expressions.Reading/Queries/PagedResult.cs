using Raiqub.Expressions.Queries.Internal;

namespace Raiqub.Expressions.Queries;

/// <summary>Represents the result of paged query.</summary>
/// <typeparam name="TResult">The type of items returned by the query.</typeparam>
public sealed class PagedResult<TResult>
{
    public PagedResult(long pageNumber, int pageSize, IReadOnlyList<TResult> items, long totalCount)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        Items = items;
        TotalCount = totalCount;

        PageCount = Paging.GetPageCount(pageSize, totalCount);
    }

    /// <summary>Gets current page number (one-based).</summary>
    public long PageNumber { get; }

    /// <summary>Gets page size.</summary>
    public int PageSize { get; }

    /// <summary>Gets the records found for the paged query.</summary>
    public IReadOnlyList<TResult> Items { get; }

    /// <summary>Gets the total number of records.</summary>
    public long TotalCount { get; }

    /// <summary>Gets number of available pages.</summary>
    public long PageCount { get; }

    /// <summary>Gets a value indicating whether there is a previous page.</summary>
    public bool HasPreviousPage => PageCount > 0 && PageNumber > 1;

    /// <summary>Gets a value indicating whether there is a next page.</summary>
    public bool HasNextPage => PageNumber < PageCount;

    /// <summary>Gets a value indicating whether the current page is the first page.</summary>
    public bool IsFirstPage => PageCount > 0 && PageNumber == 1;

    /// <summary>Gets a value indicating whether the current page is the last page.</summary>
    public bool IsLastPage => PageCount > 0 && PageNumber == PageCount;

    /// <summary>Gets a value indicating whether the current page exists.</summary>
    public bool IsValidPage => PageNumber <= PageCount;

    /// <summary>Gets one-based index of first item in current page.</summary>
    public long FirstItemOnPage => IsValidPage ? (PageNumber - 1L) * PageSize + 1L : 0L;

    /// <summary>Gets one-based index of last item in current page.</summary>
    public long LastItemOnPage
    {
        get
        {
            if (!IsValidPage) return 0L;
            long num = FirstItemOnPage + PageSize - 1L;
            return num > TotalCount ? TotalCount : num;
        }
    }
}
