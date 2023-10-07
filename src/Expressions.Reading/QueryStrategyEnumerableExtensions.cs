using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions;

/// <summary>
/// Provides extensions for using <see cref="IEntityQueryStrategy{TSource,TResult}"/> with <see cref="IEnumerable{T}"/>
/// and <see cref="IQueryable{T}"/> instances.
/// </summary>
public static class QueryStrategyEnumerableExtensions
{
    /// <summary>Apply the specified query strategy to the specified sequence.</summary>
    /// <param name="source">The sequence to apply the query strategy.</param>
    /// <param name="queryStrategy">The query strategy to apply to sequence.</param>
    /// <typeparam name="TSource">The type of the data source.</typeparam>
    /// <typeparam name="TResult">The type of the query result.</typeparam>
    /// <returns>A new sequence with the query strategy applied.</returns>
    public static IEnumerable<TResult> Apply<TSource, TResult>(
        this IEnumerable<TSource> source,
        IEntityQueryStrategy<TSource, TResult> queryStrategy) => queryStrategy.Execute(source);

    /// <summary>Apply the specified query strategy to the specified data source.</summary>
    /// <param name="queryable">The data source to apply the query strategy.</param>
    /// <param name="queryStrategy">The query strategy to apply to data source.</param>
    /// <typeparam name="TSource">The type of the data source.</typeparam>
    /// <typeparam name="TResult">The type of the query result.</typeparam>
    /// <returns>A new data source with the query strategy applied.</returns>
    public static IQueryable<TResult> Apply<TSource, TResult>(
        this IQueryable<TSource> queryable,
        IEntityQueryStrategy<TSource, TResult> queryStrategy) => queryStrategy.Execute(queryable);
}
