namespace Raiqub.Expressions.Queries;

public static class QueryModel
{
    /// <summary>
    /// Returns a query model for the specified type <typeparamref name="T"/> that does nothing.
    /// </summary>
    /// <typeparam name="T">The type of the elements of query model.</typeparam>
    /// <returns>A query model for the specified type <typeparamref name="T"/>.</returns>
    public static IQueryModel<T> Create<T>() =>
        AllQueryModel<T>.Instance;

    public static IQueryModel<T> Create<T>(Specification<T> specification) =>
        new SpecificationQueryModel<T>(specification);

    public static IQueryModel<TSource, TResult> Create<TSource, TResult>(
        Func<IQueryable<TSource>, IQueryable<TResult>> queryModel) =>
        new AnonymousQueryModel<TSource, TResult>(queryModel);

    /// <summary>Prepares a query model for down-casting.</summary>
    /// <typeparam name="TSource">The type of the data source.</typeparam>
    /// <typeparam name="TResult">The type of the query result.</typeparam>
    /// <param name="queryModel">The query model to downcast.</param>
    /// <returns>An instance that allow down-casting the specified query model.</returns>
    public static QueryModelDownCasting<TSource, TResult> DownCast<TSource, TResult>(
        this IQueryModel<TSource, TResult> queryModel)
        where TSource : class, TResult => new(queryModel);

    /// <summary>Prepares a query model for casting the source type.</summary>
    /// <typeparam name="TSource">The source type of the query model.</typeparam>
    /// <typeparam name="TResult">The result type of the query model.</typeparam>
    /// <param name="queryModel">The query model to cast.</param>
    /// <returns>An instance that allow casting the source type of the specified query model.</returns>
    public static QueryModelSourceCasting<TSource, TResult> SourceCast<TSource, TResult>(
        this IQueryModel<TSource, TResult> queryModel) => new(queryModel);
}
