namespace Raiqub.Expressions.Queries;

internal sealed class AnonymousQueryModel<T> : QueryModel<T>
{
    private readonly IEnumerable<Specification<T>> _specifications;

    public AnonymousQueryModel(IEnumerable<Specification<T>> specifications)
    {
        _specifications = specifications;
    }

    protected override IEnumerable<Specification<T>> GetPreconditions() => _specifications;

    protected override IQueryable<T> ExecuteCore(IQueryable<T> source) => source;
}
