namespace Raiqub.Expressions.Queries;

/// <summary>
/// Represents a query model that can execute a query on a data source of type <typeparamref name="TSource"/>
/// and return a result of type <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TSource">The type of the data source.</typeparam>
/// <typeparam name="TResult">The type of the query result.</typeparam>
public interface IEntityQueryModel<in TSource, out TResult>
{
    /// <summary>
    /// Executes the query on the specified data source of type <typeparamref name="TSource"/> and returns a query
    /// result of type <typeparamref name="TResult"/>.
    /// </summary>
    /// <param name="source">The data source to execute the query on.</param>
    /// <returns>The query result of type <typeparamref name="TResult"/>.</returns>
    IQueryable<TResult> Execute(IQueryable<TSource> source);

    /// <summary>
    /// Executes the query on the specified data source of type <typeparamref name="TSource"/> and returns an
    /// enumerable collection of type <typeparamref name="TResult"/>.
    /// </summary>
    /// <param name="source">The data source to execute the query on.</param>
    /// <returns>An enumerable collection of type <typeparamref name="TResult"/> that represents the query result.</returns>
    IEnumerable<TResult> Execute(IEnumerable<TSource> source);
}

/// <summary>
/// Represents a query model that can execute a query on a data source of type <typeparamref name="T"/> and return
/// a result of the same type.
/// </summary>
/// <typeparam name="T">The type of the data source and the query result.</typeparam>
public interface IEntityQueryModel<T> : IEntityQueryModel<T, T>
{
}
