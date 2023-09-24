namespace Raiqub.Expressions.Queries;

/// <summary>
/// Represents the operation of casting the source type of <see cref="IEntityQueryStrategy{TSource,TResult}"/>.
/// </summary>
/// <typeparam name="TSource">The type of the data source.</typeparam>
/// <typeparam name="TResult">The type of the query result.</typeparam>
public readonly ref struct QueryStrategySourceCasting<TSource, TResult>
{
    private readonly IEntityQueryStrategy<TSource, TResult> _queryStrategy;

    public QueryStrategySourceCasting(IEntityQueryStrategy<TSource, TResult> queryStrategy) => _queryStrategy = queryStrategy;

    /// <summary>Returns a query strategy with the source type casted to the specified derived type.</summary>
    /// <typeparam name="TDerived">The derived type to cast the source type to.</typeparam>
    /// <returns>A query strategy with the source type casted to the specified derived type.</returns>
    public IEntityQueryStrategy<TDerived, TResult> Of<TDerived>()
        where TDerived : class, TSource => _queryStrategy;
}
