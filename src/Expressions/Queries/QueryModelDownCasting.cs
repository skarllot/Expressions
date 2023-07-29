namespace Raiqub.Expressions.Queries;

/// <summary>Represents the operation of down-casting a <see cref="IQueryModel{TSource,TResult}"/>.</summary>
/// <typeparam name="TSource">The type of the data source.</typeparam>
/// <typeparam name="TResult">The type of the query result.</typeparam>
public readonly ref struct QueryModelDownCasting<TSource, TResult>
    where TSource : class, TResult
{
    private readonly IQueryModel<TSource, TResult> _queryModel;

    public QueryModelDownCasting(IQueryModel<TSource, TResult> queryModel) => _queryModel = queryModel;

    /// <summary>Returns a query model that has been downcasted to the source type.</summary>
    /// <returns>A query model that has been downcasted to the source type.</returns>
    public IQueryModel<TSource> Create() =>
        new DerivedQueryModel<TResult, TSource>(_queryModel);

    /// <summary>Returns a query model that has been downcasted to the specified derived type.</summary>
    /// <typeparam name="TDerived">The derived type of the query model.</typeparam>
    /// <returns>A query model that has been downcasted to the specified derived type.</returns>
    public IQueryModel<TDerived> To<TDerived>()
        where TDerived : class, TSource =>
        new DerivedQueryModel<TResult, TDerived>(_queryModel);
}
