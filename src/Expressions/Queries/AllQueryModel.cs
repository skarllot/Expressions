namespace Raiqub.Expressions.Queries;

internal sealed class AllQueryModel<T> : IQueryModel<T>
{
    public static readonly IQueryModel<T> Instance = new AllQueryModel<T>();

    public IQueryable<T> Execute(IQueryable<T> source) => source;

    public IEnumerable<T> Execute(IEnumerable<T> source) => source;
}
