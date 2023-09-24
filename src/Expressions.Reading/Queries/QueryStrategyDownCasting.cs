using Raiqub.Expressions.Queries.Internal;

namespace Raiqub.Expressions.Queries;

/// <summary>Represents the operation of down-casting a <see cref="IEntityQueryStrategy{TSource,TResult}"/>.</summary>
/// <typeparam name="TSource">The type of the data source.</typeparam>
/// <typeparam name="TResult">The type of the query result.</typeparam>
public readonly ref struct QueryStrategyDownCasting<TSource, TResult>
    where TSource : class, TResult
{
    private readonly IEntityQueryStrategy<TSource, TResult> _queryStrategy;

    public QueryStrategyDownCasting(IEntityQueryStrategy<TSource, TResult> queryStrategy) => _queryStrategy = queryStrategy;

    /// <summary>Returns a query strategy that has been downcasted to the source type.</summary>
    /// <returns>A query strategy that has been downcasted to the source type.</returns>
    public IEntityQueryStrategy<TSource> Create() =>
        new DerivedEntityQueryStrategy<TResult, TSource>(_queryStrategy);

    /// <summary>Returns a query strategy that has been downcasted to the specified derived type.</summary>
    /// <typeparam name="TDerived">The derived type of the query strategy.</typeparam>
    /// <returns>A query strategy that has been downcasted to the specified derived type.</returns>
    public IEntityQueryStrategy<TDerived> To<TDerived>()
        where TDerived : class, TSource =>
        new DerivedEntityQueryStrategy<TResult, TDerived>(_queryStrategy);
}
