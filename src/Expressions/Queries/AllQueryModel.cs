namespace Raiqub.Expressions.Queries;

internal sealed class AllQueryModel<T> : QueryModel<T>
{
    public static readonly QueryModel<T> Instance = new AllQueryModel<T>();

    protected override IEnumerable<Specification<T>> GetPreconditions() => Enumerable.Empty<Specification<T>>();

    protected override IQueryable<T> ExecuteCore(IQueryable<T> source) => source;
}
