namespace Raiqub.Specification;

public static class EnumerableExtensions
{
    public static IEnumerable<TResult> Query<TSource, TResult>(
        this IEnumerable<TSource> source,
        Query<TSource, TResult> query) => query.Execute(source);

    public static IQueryable<TResult> Query<TSource, TResult>(
        this IQueryable<TSource> queryable,
        Query<TSource, TResult> query) => query.Execute(queryable);

    public static IEnumerable<T> Where<T>(this IEnumerable<T> source, Specification<T> specification) =>
        source.Where(specification.IsSatisfiedBy);

    public static IQueryable<T> Where<T>(this IQueryable<T> queryable, Specification<T> specification) =>
        queryable.Where(specification.ToExpression());
}
