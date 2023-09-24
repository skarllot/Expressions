namespace Raiqub.Expressions.Queries.Internal;

internal sealed class EntityQueryStrategyWrapper<TSource, TResult> : IQueryStrategy<TResult>
    where TSource : class
{
    private readonly IEntityQueryStrategy<TSource, TResult> _entityQueryStrategy;

    public EntityQueryStrategyWrapper(IEntityQueryStrategy<TSource, TResult> entityQueryStrategy)
    {
        _entityQueryStrategy = entityQueryStrategy;
    }

    public IQueryable<TResult> Execute(IQuerySource source) => source.GetSetUsing(_entityQueryStrategy);
}
