using Raiqub.Expressions.Queries.Internal;

namespace Raiqub.Expressions.Queries;

public static class EntiyQueryModelExtensions
{
    /// <summary>Prepares a query model for down-casting.</summary>
    /// <typeparam name="TSource">The type of the data source.</typeparam>
    /// <typeparam name="TResult">The type of the query result.</typeparam>
    /// <param name="queryModel">The query model to downcast.</param>
    /// <returns>An instance that allow down-casting the specified query model.</returns>
    public static QueryModelDownCasting<TSource, TResult> DownCast<TSource, TResult>(
        this IEntityQueryModel<TSource, TResult> queryModel)
        where TSource : class, TResult => new(queryModel);

    /// <summary>Prepares a query model for casting the source type.</summary>
    /// <typeparam name="TSource">The source type of the query model.</typeparam>
    /// <typeparam name="TResult">The result type of the query model.</typeparam>
    /// <param name="queryModel">The query model to cast.</param>
    /// <returns>An instance that allow casting the source type of the specified query model.</returns>
    public static QueryModelSourceCasting<TSource, TResult> SourceCast<TSource, TResult>(
        this IEntityQueryModel<TSource, TResult> queryModel) => new(queryModel);

    /// <summary>
    /// Convert a <see cref="IEntityQueryModel{TSource,TResult}"/> to <see cref="IQueryModel{TResult}"/>.
    /// </summary>
    /// <param name="queryModel">The entity query model to convert.</param>
    /// <typeparam name="TSource">The source type of the query model.</typeparam>
    /// <typeparam name="TResult">The result type of the query model.</typeparam>
    /// <returns>An instance of <see cref="IQueryModel{TResult}"/> representing the exact query model as specified by <paramref name="queryModel"/>.</returns>
    public static IQueryModel<TResult> ToQueryModel<TSource, TResult>(
        this IEntityQueryModel<TSource, TResult> queryModel) where TSource : class =>
        queryModel as IQueryModel<TResult> ?? new EntityQueryModelWrapper<TSource, TResult>(queryModel);
}
