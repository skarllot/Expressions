namespace Raiqub.Expressions.Queries;

public interface IQueryModel<in TSource, out TResult>
{
    IQueryable<TResult> Execute(IQueryable<TSource> source);
    IEnumerable<TResult> Execute(IEnumerable<TSource> source);

#if !NETSTANDARD2_0
    IQueryModel<TDerived, TResult> SourceAs<TDerived>() where TDerived : class, TSource => this;
#endif
}

public interface IQueryModel<T> : IQueryModel<T, T>
{
#if !NETSTANDARD2_0
    IQueryModel<TDerived> DownCast<TDerived>() where TDerived : class, T => new DerivedQueryModel<T, TDerived>(this);
#endif
}
