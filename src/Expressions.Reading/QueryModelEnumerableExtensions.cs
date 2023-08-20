using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions;

public static class QueryModelEnumerableExtensions
{
    public static IEnumerable<TResult> Apply<TSource, TResult>(
        this IEnumerable<TSource> source,
        IEntityQueryModel<TSource, TResult> queryModel) => queryModel.Execute(source);

    public static IQueryable<TResult> Apply<TSource, TResult>(
        this IQueryable<TSource> queryable,
        IEntityQueryModel<TSource, TResult> queryModel) => queryModel.Execute(queryable);
}
