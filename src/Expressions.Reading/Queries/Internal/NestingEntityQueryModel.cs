using System.Linq.Expressions;

namespace Raiqub.Expressions.Queries.Internal;

internal sealed class NestingEntityQueryModel<TEntity, TNested, TResult>
    : IEntityQueryModel<TEntity, TResult>, IQueryModel<TResult>
    where TEntity : class
{
    private readonly Expression<Func<TEntity, IEnumerable<TNested>>> _selector;
    private readonly IEntityQueryModel<TNested, TResult> _nestedQueryModel;

    public NestingEntityQueryModel(
        Expression<Func<TEntity, IEnumerable<TNested>>> selector,
        IEntityQueryModel<TNested, TResult> nestedQueryModel)
    {
        _selector = selector;
        _nestedQueryModel = nestedQueryModel;
    }

    public IQueryable<TResult> Execute(IQueryable<TEntity> source) =>
        _nestedQueryModel.Execute(source.SelectMany(_selector));

    public IEnumerable<TResult> Execute(IEnumerable<TEntity> source) =>
        _nestedQueryModel.Execute(source.SelectMany(_selector.Compile()));

    public IQueryable<TResult> Execute(IQuerySource source) => Execute(source.GetSet<TEntity>());
}
