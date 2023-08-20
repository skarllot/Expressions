namespace Raiqub.Expressions;

public static class SpecificationEnumerableExtensions
{
    public static IEnumerable<T> Where<T>(this IEnumerable<T> source, Specification<T> specification) =>
        source.Where(specification.IsSatisfiedBy);

    public static IQueryable<T> Where<T>(this IQueryable<T> queryable, Specification<T> specification) =>
        queryable.Where(specification.ToExpression());
}
