namespace Raiqub.Expressions.Queries;

internal sealed class DerivedQueryModel<TParent, TDerived> : IQueryModel<TDerived>
    where TDerived : class, TParent
{
    private readonly IQueryModel<TDerived, TParent> _originalQueryModel;

    public DerivedQueryModel(IQueryModel<TDerived, TParent> originalQueryModel) =>
        _originalQueryModel = originalQueryModel;

    public IQueryable<TDerived> Execute(IQueryable<TDerived> source) =>
        _originalQueryModel.Execute(source).OfType<TDerived>();

    public IEnumerable<TDerived> Execute(IEnumerable<TDerived> source) =>
        _originalQueryModel.Execute(source).OfType<TDerived>();
}
