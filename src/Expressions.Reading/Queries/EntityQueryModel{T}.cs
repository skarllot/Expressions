namespace Raiqub.Expressions.Queries;

public abstract class EntityQueryModel<T> : EntityQueryModel<T, T>, IEntityQueryModel<T>
    where T : class
{
    protected override IQueryable<T> ExecuteCore(IQueryable<T> source) => source;
}
