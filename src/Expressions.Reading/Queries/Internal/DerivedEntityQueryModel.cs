namespace Raiqub.Expressions.Queries.Internal;

internal sealed class DerivedEntityQueryModel<TParent, TDerived>
    : IEntityQueryModel<TDerived>, IQueryModel<TDerived>
    where TDerived : class, TParent
{
    private readonly IEntityQueryModel<TDerived, TParent> _originalQueryModel;

    public DerivedEntityQueryModel(IEntityQueryModel<TDerived, TParent> originalQueryModel) =>
        _originalQueryModel = originalQueryModel;

    public IQueryable<TDerived> Execute(IQueryable<TDerived> source) =>
        _originalQueryModel.Execute(source).OfType<TDerived>();

    public IEnumerable<TDerived> Execute(IEnumerable<TDerived> source) =>
        _originalQueryModel.Execute(source).OfType<TDerived>();

    public IQueryable<TDerived> Execute(IQuerySource source) =>
        Execute(source.GetSet<TDerived>());
}
