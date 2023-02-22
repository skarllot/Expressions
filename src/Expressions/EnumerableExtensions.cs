using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions;

public static class EnumerableExtensions
{
    public static IEnumerable<TResult> Apply<TSource, TResult>(
        this IEnumerable<TSource> source,
        QueryModel<TSource, TResult> queryModel) => queryModel.Execute(source);

    public static IQueryable<TResult> Apply<TSource, TResult>(
        this IQueryable<TSource> queryable,
        QueryModel<TSource, TResult> queryModel) => queryModel.Execute(queryable);

    public static IEnumerable<T> Where<T>(this IEnumerable<T> source, Specification<T> specification) =>
        source.Where(specification.IsSatisfiedBy);

    public static IQueryable<T> Where<T>(this IQueryable<T> queryable, Specification<T> specification) =>
        queryable.Where(specification.ToExpression());
}
