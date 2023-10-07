namespace Raiqub.Expressions.Queries.Paging;

/// <summary>Represents the information of a queried page.</summary>
public readonly struct PageInfo
{
    /// <summary>The current page number (one-based).</summary>
    public readonly long PageNumber;

    /// <summary>The page size.</summary>
    public readonly int PageSize;

    /// <summary>The total number of records.</summary>
    public readonly long TotalCount;

    /// <summary>The number of available pages.</summary>
    public readonly long PageCount;

    /// <summary>Initializes a new instance of the <see cref="PageInfo"/> class.</summary>
    /// <param name="pageNumber">The current page number (one-based).</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="totalCount">The total number of records.</param>
    public PageInfo(long pageNumber, int pageSize, long totalCount)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;

        PageCount = GetPageCount(pageSize, totalCount);
    }

    /// <summary>Gets a value indicating whether there is a previous page.</summary>
    public bool HasPreviousPage => PageCount > 0 && PageNumber > 1;

    /// <summary>Gets a value indicating whether there is a next page.</summary>
    public bool HasNextPage => PageNumber < PageCount;

    /// <summary>Gets a value indicating whether the current page is the first page.</summary>
    public bool IsFirstPage => PageCount > 0 && PageNumber == 1;

    /// <summary>Gets a value indicating whether the current page is the last page.</summary>
    public bool IsLastPage => PageCount > 0 && PageNumber == PageCount;

    /// <summary>Gets one-based index of first item in current page.</summary>
    public long FirstItemOnPage => PageCount > 0 ? (PageNumber - 1L) * PageSize + 1L : 0L;

    /// <summary>
    /// Determines whether the specified page number exists given the page size and record count.
    /// </summary>
    /// <param name="pageNumber">The one-based page number to retrieve.</param>
    /// <param name="pageSize">The maximum number of elements to return.</param>
    /// <param name="totalCount">The total number of records.</param>
    /// <returns>True whether the specified page number exists; otherwise, false.</returns>
    public static bool PageNumberExists(int pageNumber, int pageSize, long totalCount)
    {
        long pageCount = GetPageCount(pageSize, totalCount);
        return pageNumber <= pageCount;
    }

    /// <summary>Gets one-based index of last item in current page.</summary>
    /// <param name="actualItemsCount">The number of items returned by the query.</param>
    public long GetLastItemOnPage(int actualItemsCount) =>
        actualItemsCount > 0 ? FirstItemOnPage + actualItemsCount - 1L : 0L;

    private static long GetPageCount(int pageSize, long totalCount) =>
        totalCount > 0L ? (long)Math.Ceiling(totalCount / (double)pageSize) : 0L;
}
