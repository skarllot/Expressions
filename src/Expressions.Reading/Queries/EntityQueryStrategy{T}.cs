namespace Raiqub.Expressions.Queries;

public abstract class EntityQueryStrategy<TSource>
    : EntityQueryStrategy<TSource, TSource>, IEntityQueryStrategy<TSource>
    where TSource : class
{
    protected EntityQueryStrategy()
    {
    }

    protected EntityQueryStrategy(params Specification<TSource>[] restrictions) : base(restrictions)
    {
    }

    protected EntityQueryStrategy(IEnumerable<Specification<TSource>> restrictions) : base(restrictions)
    {
    }

    protected override IQueryable<TSource> ExecuteCore(IQueryable<TSource> source) => source;
}
