namespace Raiqub.Expressions.Queries.Internal;

public static class Paging
{
    public static bool PageNumberExists(int pageNumber, int pageSize, long totalCount)
    {
        long pageCount = GetPageCount(pageSize, totalCount);
        return pageNumber <= pageCount;
    }

    public static long GetPageCount(int pageSize, long totalCount)
    {
        return totalCount > 0L ? (long)Math.Ceiling(totalCount / (double)pageSize) : 0L;
    }
}
