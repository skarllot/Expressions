namespace Raiqub.Expressions.Queries;

public abstract class EntityQueryStrategy<TSource>
    : EntityQueryStrategy<TSource, TSource>, IEntityQueryStrategy<TSource>
    where TSource : class
{
    protected override IQueryable<TSource> ExecuteCore(IQueryable<TSource> source) => source;
}
