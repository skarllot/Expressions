using Raiqub.Expressions.Queries.Internal;

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
        this IEntityQueryStrategy<TSource, TResult> queryStrategy
    )
        where TSource : class, TResult => new(queryStrategy);

    /// <summary>Prepares a query strategy for casting the source type.</summary>
    /// <typeparam name="TSource">The source type of the query strategy.</typeparam>
    /// <typeparam name="TResult">The result type of the query strategy.</typeparam>
    /// <param name="queryStrategy">The query strategy to cast.</param>
    /// <returns>An instance that allow casting the source type of the specified query strategy.</returns>
    public static QueryStrategySourceCasting<TSource, TResult> SourceCast<TSource, TResult>(
        this IEntityQueryStrategy<TSource, TResult> queryStrategy
    ) => new(queryStrategy);

    /// <summary>Chains two query strategies together, executing them sequentially.</summary>
    /// <typeparam name="TSource">The type of the source data.</typeparam>
    /// <typeparam name="TIntermediate">The intermediate type between the two query strategies.</typeparam>
    /// <typeparam name="TResult">The type of the final result.</typeparam>
    /// <param name="first">The first query strategy to execute.</param>
    /// <param name="second">The second query strategy to execute on the result of the first.</param>
    /// <returns>A combined query strategy that executes both strategies in sequence.</returns>
    public static IEntityQueryStrategy<TSource, TResult> Then<TSource, TIntermediate, TResult>(
        this IEntityQueryStrategy<TSource, TIntermediate> first,
        IEntityQueryStrategy<TIntermediate, TResult> second
    ) => new CombinedQueryStrategy<TSource, TIntermediate, TResult>(first, second);

    /// <summary>Chains a query strategy with an entity query strategy, executing them sequentially.</summary>
    /// <typeparam name="TIntermediate">The intermediate type produced by the first strategy.</typeparam>
    /// <typeparam name="TResult">The type of the final result.</typeparam>
    /// <param name="first">The first query strategy to execute against the query source.</param>
    /// <param name="second">The second entity query strategy to execute on the result of the first.</param>
    /// <returns>A combined query strategy that executes both strategies in sequence.</returns>
    public static IQueryStrategy<TResult> Then<TIntermediate, TResult>(
        this IQueryStrategy<TIntermediate> first,
        IEntityQueryStrategy<TIntermediate, TResult> second
    ) => new CombinedQueryStrategy<TIntermediate, TResult>(first, second);
}
