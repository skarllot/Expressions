using Microsoft.Extensions.Logging;

namespace Raiqub.Expressions.Queries;

internal static class QueryLog
{
    private static readonly Action<ILogger, Exception?> s_anyErrorCallback = LoggerMessage.Define(
        LogLevel.Error,
        0,
        "Error trying to query whether any element exists");

    private static readonly Action<ILogger, Exception?> s_countErrorCallback = LoggerMessage.Define(
        LogLevel.Error,
        1,
        "Error trying to count elements");

    private static readonly Action<ILogger, Exception?> s_firstErrorCallback = LoggerMessage.Define(
        LogLevel.Error,
        2,
        "Error trying to query the first element");

    private static readonly Action<ILogger, Exception?> s_listErrorCallback = LoggerMessage.Define(
        LogLevel.Error,
        3,
        "Error trying to list found elements");

    private static readonly Action<ILogger, Exception?> s_singleErrorCallback = LoggerMessage.Define(
        LogLevel.Error,
        4,
        "Error trying to query the single element");

    private static readonly Action<ILogger, Exception?> s_asyncEnumerableErrorCallback = LoggerMessage.Define(
        LogLevel.Error,
        5,
        "Error trying to enumerate found elements");

    private static readonly Action<ILogger, Exception?> s_pagedListErrorCallback = LoggerMessage.Define(
        LogLevel.Error,
        6,
        "Error trying to query a page of found elements");

    public static void AnyError(ILogger logger, Exception exception) => s_anyErrorCallback(logger, exception);
    public static void CountError(ILogger logger, Exception exception) => s_countErrorCallback(logger, exception);
    public static void FirstError(ILogger logger, Exception exception) => s_firstErrorCallback(logger, exception);
    public static void PagedListError(ILogger logger, Exception exception) => s_pagedListErrorCallback(logger, exception);
    public static void ListError(ILogger logger, Exception exception) => s_listErrorCallback(logger, exception);
    public static void SingleError(ILogger logger, Exception exception) => s_singleErrorCallback(logger, exception);

    public static void AsyncEnumerableError(ILogger logger, Exception exception) =>
        s_asyncEnumerableErrorCallback(logger, exception);
}
