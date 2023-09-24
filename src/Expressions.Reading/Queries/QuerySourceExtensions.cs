namespace Raiqub.Expressions.Queries;

/// <summary>Provides extensions for <see cref="IQuerySource"/> interface.</summary>
public static class QuerySourceExtensions
{
    /// <summary>Gets the data source using the query defined by the specified entity query strategy.</summary>
    /// <param name="querySource">The provider to get data source from.</param>
    /// <param name="queryStrategy">The query strategy to apply to the data source.</param>
    /// <typeparam name="TSource">The type of the data source.</typeparam>
    /// <typeparam name="TResult">The type of the query result.</typeparam>
    /// <returns>The data source of type <typeparamref name="TSource"/> using the specified query strategy.</returns>
    public static IQueryable<TResult> GetSetUsing<TSource, TResult>(
        this IQuerySource querySource,
        IEntityQueryStrategy<TSource, TResult> queryStrategy)
        where TSource : class
    {
        return queryStrategy.Execute(querySource);
    }

    /// <summary>Gets the data source applying the specified criteria.</summary>
    /// <param name="querySource">The provider to get data source from.</param>
    /// <param name="specification">The specification to apply to the data source.</param>
    /// <typeparam name="TSource">The type of the data source.</typeparam>
    /// <returns>The data source of type <typeparamref name="TSource"/> returning data that matches the specified criteria.</returns>
    public static IQueryable<TSource> GetSetUsing<TSource>(
        this IQuerySource querySource,
        Specification<TSource> specification)
        where TSource : class
    {
        return querySource.GetSet<TSource>().Where(specification);
    }
}
