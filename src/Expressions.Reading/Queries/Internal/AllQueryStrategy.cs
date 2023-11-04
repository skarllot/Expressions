namespace Raiqub.Expressions.Queries.Internal;

internal sealed class AllQueryStrategy<T> : IEntityQueryStrategy<T>
    where T : class
{
    public static readonly AllQueryStrategy<T> Instance = new();

    public IQueryable<T> Execute(IQueryable<T> source) => source;

    public IEnumerable<T> Execute(IEnumerable<T> source) => source;
}
