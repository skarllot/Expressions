namespace Raiqub.Expressions.Queries;

internal sealed class AllQuery<T> : Query<T>
{
    public static readonly Query<T> Instance = new AllQuery<T>();

    protected override IEnumerable<Specification<T>> GetPreconditions() => Enumerable.Empty<Specification<T>>();

    protected override IQueryable<T> ExecuteCore(IQueryable<T> source) => source;
}
