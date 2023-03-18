namespace Raiqub.Expressions.Queries;

public abstract class QueryModel<TSource, TResult> : IQueryModel<TSource, TResult>
{
    private readonly IEnumerable<Specification<TSource>> _restrictions;

    protected QueryModel()
    {
        _restrictions = Enumerable.Empty<Specification<TSource>>();
    }

    protected QueryModel(params Specification<TSource>[] restrictions)
        : this((IEnumerable<Specification<TSource>>)restrictions)
    {
    }

    protected QueryModel(IEnumerable<Specification<TSource>> restrictions)
    {
        _restrictions = restrictions.ToList();
    }

    private Specification<TSource> CombinedSpecification =>
        Specification.And(GetPreconditions().Concat(_restrictions));

    public IQueryable<TResult> Execute(IQueryable<TSource> source)
    {
        return ExecuteCore(source.Where(CombinedSpecification));
    }

    public IEnumerable<TResult> Execute(IEnumerable<TSource> source)
    {
        return Execute(source.AsQueryable());
    }

    protected abstract IEnumerable<Specification<TSource>> GetPreconditions();
    protected abstract IQueryable<TResult> ExecuteCore(IQueryable<TSource> source);
}
