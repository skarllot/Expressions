namespace Raiqub.Expressions.Queries;

public interface IQueryModel<in TSource, out TResult>
{
    IQueryable<TResult> Execute(IQueryable<TSource> source);
    IEnumerable<TResult> Execute(IEnumerable<TSource> source);
}

public interface IQueryModel<T> : IQueryModel<T, T>
{
}
