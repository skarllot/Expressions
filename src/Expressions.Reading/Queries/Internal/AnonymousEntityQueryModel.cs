namespace Raiqub.Expressions.Queries.Internal;

internal sealed class AnonymousEntityQueryModel<TSource, TResult>
    : IEntityQueryModel<TSource, TResult>, IQueryModel<TResult>
    where TSource : class
{
    private readonly Func<IQueryable<TSource>, IQueryable<TResult>> _queryModel;

    public AnonymousEntityQueryModel(Func<IQueryable<TSource>, IQueryable<TResult>> queryModel)
    {
        _queryModel = queryModel;
    }

    public IQueryable<TResult> Execute(IQueryable<TSource> source) => _queryModel(source);

    public IEnumerable<TResult> Execute(IEnumerable<TSource> source) => _queryModel(source.AsQueryable());

    public IQueryable<TResult> Execute(IQuerySource source) => _queryModel(source.GetSet<TSource>());
}
