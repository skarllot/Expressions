namespace Raiqub.Expressions.Queries.Internal;

internal sealed class AnonymousEntityQueryStrategy<TSource, TResult>
    : IEntityQueryStrategy<TSource, TResult>
    where TSource : class
{
    private readonly Func<IQueryable<TSource>, IQueryable<TResult>> _strategy;

    public AnonymousEntityQueryStrategy(Func<IQueryable<TSource>, IQueryable<TResult>> strategy)
    {
        _strategy = strategy;
    }

    public IQueryable<TResult> Execute(IQueryable<TSource> source) => _strategy(source);

    public IEnumerable<TResult> Execute(IEnumerable<TSource> source) => _strategy(source.AsQueryable());
}
