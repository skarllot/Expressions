using System.Linq.Expressions;

namespace Raiqub.Expressions.Queries;

internal sealed class NestingQueryModel<TEntity, TNested, TResult> : IQueryModel<TEntity, TResult>
{
    private readonly Expression<Func<TEntity, IEnumerable<TNested>>> _selector;
    private readonly IQueryModel<TNested, TResult> _nestedQueryModel;

    public NestingQueryModel(
        Expression<Func<TEntity, IEnumerable<TNested>>> selector,
        IQueryModel<TNested, TResult> nestedQueryModel)
    {
        _selector = selector;
        _nestedQueryModel = nestedQueryModel;
    }

    public IQueryable<TResult> Execute(IQueryable<TEntity> source) =>
        _nestedQueryModel.Execute(source.SelectMany(_selector));

    public IEnumerable<TResult> Execute(IEnumerable<TEntity> source) =>
        _nestedQueryModel.Execute(source.SelectMany(_selector.Compile()));
}
