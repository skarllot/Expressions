namespace Raiqub.Expressions.Queries.Internal;

internal sealed class AllQueryModel<T> : IEntityQueryModel<T>, IQueryModel<T>
    where T : class
{
    public static readonly AllQueryModel<T> Instance = new AllQueryModel<T>();

    public IQueryable<T> Execute(IQueryable<T> source) => source;

    public IEnumerable<T> Execute(IEnumerable<T> source) => source;

    public IQueryable<T> Execute(IQuerySource source) => source.GetSet<T>();
}
