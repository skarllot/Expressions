namespace Raiqub.Expressions.Queries.Internal;

internal sealed class CombinedQueryStrategy<TSource, TIntermediate, TResult>(
    IEntityQueryStrategy<TSource, TIntermediate> first,
    IEntityQueryStrategy<TIntermediate, TResult> second
) : IEntityQueryStrategy<TSource, TResult>
{
    public IQueryable<TResult> Execute(IQueryable<TSource> source) => second.Execute(first.Execute(source));

    public IEnumerable<TResult> Execute(IEnumerable<TSource> source) => second.Execute(first.Execute(source));
}

internal sealed class CombinedQueryStrategy<TIntermediate, TResult>(
    IQueryStrategy<TIntermediate> first,
    IEntityQueryStrategy<TIntermediate, TResult> second
) : IQueryStrategy<TResult>
{
    public IQueryable<TResult> Execute(IQuerySource source) => second.Execute(first.Execute(source));
}
