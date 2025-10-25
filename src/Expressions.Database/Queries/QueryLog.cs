using Microsoft.Extensions.Logging;

namespace Raiqub.Expressions.Queries;

internal static class QueryLog
{
    private static readonly Action<ILogger, int, Exception?> s_listCountInfoCallback =
        LoggerMessage.Define<int>(LogLevel.Information, 7, "The query returned {ItemCount} items");

    private static readonly Action<ILogger, int, long, Exception?> s_pagedListCountInfoCallback =
        LoggerMessage.Define<int, long>(
            LogLevel.Information,
            8,
            "The query returned {ItemCount} items of {TotalCount}"
        );

    private static readonly Action<ILogger, long, Exception?> s_pagedListInvalidPageCallback =
        LoggerMessage.Define<long>(
            LogLevel.Warning,
            9,
            "The query returned no items of {TotalCount} because of invalid paging"
        );

    public static void ListCountInfo(ILogger logger, int itemCount) =>
        s_listCountInfoCallback(logger, itemCount, null);

    public static void PagedListCountInfo(ILogger logger, int itemCount, long totalCount) =>
        s_pagedListCountInfoCallback(logger, itemCount, totalCount, null);

    public static void PagedListInvalidPage(ILogger logger, long totalCount) =>
        s_pagedListInvalidPageCallback(logger, totalCount, null);
}
