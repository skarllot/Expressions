using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions;

public static class QueryStrategyEnumerableExtensions
{
    public static IEnumerable<TResult> Apply<TSource, TResult>(
        this IEnumerable<TSource> source,
        IEntityQueryStrategy<TSource, TResult> queryStrategy) => queryStrategy.Execute(source);

    public static IQueryable<TResult> Apply<TSource, TResult>(
        this IQueryable<TSource> queryable,
        IEntityQueryStrategy<TSource, TResult> queryStrategy) => queryStrategy.Execute(queryable);
}
