namespace Raiqub.Expressions.Queries.Internal;

internal sealed class AnonymousQueryModel<TResult> : IQueryModel<TResult>
{
    private readonly Func<IQuerySource, IQueryable<TResult>> _queryModel;

    public AnonymousQueryModel(Func<IQuerySource, IQueryable<TResult>> queryModel) => _queryModel = queryModel;

    public IQueryable<TResult> Execute(IQuerySource source) => _queryModel(source);
}
