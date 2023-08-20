namespace Raiqub.Expressions.Queries.Internal;

internal sealed class EntityQueryModelWrapper<TSource, TResult> : IQueryModel<TResult>
    where TSource : class
{
    private readonly IEntityQueryModel<TSource, TResult> _entityQueryModel;

    public EntityQueryModelWrapper(IEntityQueryModel<TSource, TResult> entityQueryModel)
    {
        _entityQueryModel = entityQueryModel;
    }

    public IQueryable<TResult> Execute(IQuerySource source) => source.GetSetUsing(_entityQueryModel);
}
