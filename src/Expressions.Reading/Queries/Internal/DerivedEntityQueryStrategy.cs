namespace Raiqub.Expressions.Queries.Internal;

internal sealed class DerivedEntityQueryStrategy<TParent, TDerived>
    : IEntityQueryStrategy<TDerived>
    where TDerived : class, TParent
{
    private readonly IEntityQueryStrategy<TDerived, TParent> _originalQueryStrategy;

    public DerivedEntityQueryStrategy(IEntityQueryStrategy<TDerived, TParent> originalQueryStrategy) =>
        _originalQueryStrategy = originalQueryStrategy;

    public IQueryable<TDerived> Execute(IQueryable<TDerived> source) =>
        _originalQueryStrategy.Execute(source).OfType<TDerived>();

    public IEnumerable<TDerived> Execute(IEnumerable<TDerived> source) =>
        _originalQueryStrategy.Execute(source).OfType<TDerived>();

    public IQueryable<TDerived> Execute(IQuerySource source) =>
        Execute(source.GetSet<TDerived>());
}
