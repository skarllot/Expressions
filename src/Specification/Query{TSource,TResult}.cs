namespace Raiqub.Specification;

public abstract class Query<TSource, TResult>
{
    private readonly IEnumerable<Specification<TSource>> _restrictions;

    protected Query()
    {
        _restrictions = Enumerable.Empty<Specification<TSource>>();
    }

    protected Query(params Specification<TSource>[] restrictions)
        : this((IEnumerable<Specification<TSource>>)restrictions)
    {
    }

    protected Query(IEnumerable<Specification<TSource>> restrictions)
    {
        _restrictions = restrictions.ToList();
    }

    public virtual ChangeTracking DefaultChangeTracking => ChangeTracking.Default;

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
