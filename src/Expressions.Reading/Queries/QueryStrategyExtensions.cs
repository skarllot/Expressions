namespace Raiqub.Expressions.Queries;

/// <summary>Provides extensions for <see cref="IEntityQueryStrategy{TSource,TResult}"/> instances.</summary>
public static class QueryStrategyExtensions
{
    /// <summary>Prepares a query strategy for down-casting.</summary>
    /// <typeparam name="TSource">The type of the data source.</typeparam>
    /// <typeparam name="TResult">The type of the query result.</typeparam>
    /// <param name="queryStrategy">The query strategy to downcast.</param>
    /// <returns>An instance that allow down-casting the specified query strategy.</returns>
    public static QueryStrategyDownCasting<TSource, TResult> DownCast<TSource, TResult>(
        this IEntityQueryStrategy<TSource, TResult> queryStrategy)
        where TSource : class, TResult => new(queryStrategy);

    /// <summary>Prepares a query strategy for casting the source type.</summary>
    /// <typeparam name="TSource">The source type of the query strategy.</typeparam>
    /// <typeparam name="TResult">The result type of the query strategy.</typeparam>
    /// <param name="queryStrategy">The query strategy to cast.</param>
    /// <returns>An instance that allow casting the source type of the specified query strategy.</returns>
    public static QueryStrategySourceCasting<TSource, TResult> SourceCast<TSource, TResult>(
        this IEntityQueryStrategy<TSource, TResult> queryStrategy) => new(queryStrategy);
}
