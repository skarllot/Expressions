using System.Linq.Expressions;

namespace Raiqub.Expressions.Queries.Internal;

internal sealed class NestingEntityQueryStrategy<TEntity, TNested, TResult>
    : IEntityQueryStrategy<TEntity, TResult>
    where TEntity : class
{
    private readonly Expression<Func<TEntity, IEnumerable<TNested>>> _selector;
    private readonly IEntityQueryStrategy<TNested, TResult> _nestedQueryStrategy;

    public NestingEntityQueryStrategy(
        Expression<Func<TEntity, IEnumerable<TNested>>> selector,
        IEntityQueryStrategy<TNested, TResult> nestedQueryStrategy)
    {
        _selector = selector;
        _nestedQueryStrategy = nestedQueryStrategy;
    }

    public IQueryable<TResult> Execute(IQueryable<TEntity> source) =>
        _nestedQueryStrategy.Execute(source.SelectMany(_selector));

    public IEnumerable<TResult> Execute(IEnumerable<TEntity> source) =>
        _nestedQueryStrategy.Execute(source.SelectMany(_selector.Compile()));
}
