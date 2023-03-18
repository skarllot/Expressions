namespace Raiqub.Expressions.Queries;

public static class QueryModel
{
    public static IQueryModel<T> Create<T>() =>
        AllQueryModel<T>.Instance;

    public static IQueryModel<T> Create<T>(Specification<T> specification) =>
        new SpecificationQueryModel<T>(specification);
}
