namespace Raiqub.Expressions.Queries;

/// <summary>
/// Represents the operation of casting the source type of <see cref="IEntityQueryModel{TSource,TResult}"/>.
/// </summary>
/// <typeparam name="TSource">The type of the data source.</typeparam>
/// <typeparam name="TResult">The type of the query result.</typeparam>
public readonly ref struct QueryModelSourceCasting<TSource, TResult>
{
    private readonly IEntityQueryModel<TSource, TResult> _queryModel;

    public QueryModelSourceCasting(IEntityQueryModel<TSource, TResult> queryModel) => _queryModel = queryModel;

    /// <summary>Returns a query model with the source type casted to the specified derived type.</summary>
    /// <typeparam name="TDerived">The derived type to cast the source type to.</typeparam>
    /// <returns>A query model with the source type casted to the specified derived type.</returns>
    public IEntityQueryModel<TDerived, TResult> Of<TDerived>()
        where TDerived : class, TSource => _queryModel;
}
