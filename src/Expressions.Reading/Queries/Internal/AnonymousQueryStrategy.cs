namespace Raiqub.Expressions.Queries.Internal;

internal sealed class AnonymousQueryStrategy<TResult> : IQueryStrategy<TResult>
{
    private readonly Func<IQuerySource, IQueryable<TResult>> _strategy;

    public AnonymousQueryStrategy(Func<IQuerySource, IQueryable<TResult>> strategy) => _strategy = strategy;

    public IQueryable<TResult> Execute(IQuerySource source) => _strategy(source);
}
