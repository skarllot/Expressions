namespace Raiqub.Expressions.Queries;

public static class QueryModel
{
    public static QueryModel<T> Create<T>() =>
        AllQueryModel<T>.Instance;

    public static QueryModel<T> Create<T>(Specification<T> specification) =>
        new AnonymousQueryModel<T>(new[] { specification });
}
