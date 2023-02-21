namespace Raiqub.Expressions.Queries;

public static class Query
{
    public static Query<T> Create<T>() =>
        AllQuery<T>.Instance;

    public static Query<T> Create<T>(Specification<T> specification) =>
        new AnonymousQuery<T>(new[] { specification });
}
