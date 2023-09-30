namespace Raiqub.Expressions.Queries;

internal static class PagedQuery
{
    public static IQueryable<T> PrepareQueryForPaging<T>(this IQueryable<T> queryable, int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pageNumber),
                $"Page number must be greater than or equals to 1, but it is {pageNumber}");
        }

        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pageSize),
                $"Page size must be greater than or equals to 1, but it is {pageSize}");
        }

        return pageNumber != 1
            ? queryable.Skip((pageNumber - 1) * pageSize).Take(pageSize)
            : queryable.Take(pageSize);
    }
}
