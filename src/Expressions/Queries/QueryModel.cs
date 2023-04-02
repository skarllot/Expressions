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

    /// <summary>Returns a query model that has been downcasted to the specified derived type.</summary>
    /// <typeparam name="TSource">The source type of the query model.</typeparam>
    /// <typeparam name="TDerived">The derived type of the query model.</typeparam>
    /// <param name="queryModel">The query model to downcast.</param>
    /// <returns>A query model that has been downcasted to the specified derived type.</returns>
    public static IQueryModel<TDerived> DownCast<TSource, TDerived>(this IQueryModel<TDerived, TSource> queryModel)
        where TDerived : class, TSource => new DerivedQueryModel<TSource, TDerived>(queryModel);

    /// <summary>Returns a query model that has been downcasted to the specified derived type.</summary>
    /// <typeparam name="TSource">The source type of the query model.</typeparam>
    /// <typeparam name="TDerived">The derived type of the query model.</typeparam>
    /// <param name="queryModel">The query model to downcast.</param>
    /// <param name="typeParam">The type parameter for the derived type.</param>
    /// <returns>A query model that has been downcasted to the specified derived type.</returns>
    public static IQueryModel<TDerived> DownCast<TSource, TDerived>(
        this IQueryModel<TDerived, TSource> queryModel,
        ImplicitGenerics.ITypeParam<TDerived> typeParam)
        where TDerived : class, TSource => new DerivedQueryModel<TSource, TDerived>(queryModel);

    /// <summary>Returns a query model with the source type casted to the specified derived type.</summary>
    /// <typeparam name="TSource">The source type of the query model.</typeparam>
    /// <typeparam name="TDerived">The derived type to cast the source type to.</typeparam>
    /// <typeparam name="TResult">The result type of the query model.</typeparam>
    /// <param name="queryModel">The query model to cast.</param>
    /// <returns>A query model with the source type casted to the specified derived type.</returns>
    public static IQueryModel<TDerived, TResult> SourceAs<TSource, TDerived, TResult>(
        this IQueryModel<TSource, TResult> queryModel)
        where TDerived : class, TSource => queryModel;

    /// <summary>Returns a query model with the source type casted to the specified derived type.</summary>
    /// <typeparam name="TSource">The source type of the query model.</typeparam>
    /// <typeparam name="TDerived">The derived type to cast the source type to.</typeparam>
    /// <typeparam name="TResult">The result type of the query model.</typeparam>
    /// <param name="queryModel">The query model to cast.</param>
    /// <param name="typeParam">The type parameter for the derived type.</param>
    /// <returns>A query model with the source type casted to the specified derived type.</returns>
    public static IQueryModel<TDerived, TResult> SourceAs<TSource, TDerived, TResult>(
        this IQueryModel<TSource, TResult> queryModel,
        ImplicitGenerics.ITypeParam<TDerived> typeParam)
        where TDerived : class, TSource => queryModel;
}
