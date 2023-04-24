namespace Raiqub.Expressions.Queries;

internal sealed class AnonymousQueryModel<TSource, TResult> : IQueryModel<TSource, TResult>
{
    private readonly Func<IQueryable<TSource>, IQueryable<TResult>> _queryModel;

    public AnonymousQueryModel(Func<IQueryable<TSource>, IQueryable<TResult>> queryModel)
    {
        _queryModel = queryModel;
    }

    public IQueryable<TResult> Execute(IQueryable<TSource> source) => _queryModel(source);

    public IEnumerable<TResult> Execute(IEnumerable<TSource> source) => _queryModel(source.AsQueryable());
}
