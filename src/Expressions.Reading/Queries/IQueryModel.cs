namespace Raiqub.Expressions.Queries;

/// <summary>
/// Represents a query strategy that can execute a query against one or more data sources
/// and return a result of type <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TResult">The type of the query result.</typeparam>
public interface IQueryModel<out TResult>
{
    /// <summary>
    /// Executes the query using the specified query source and
    /// returns a query result of type <typeparamref name="TResult"/>.
    /// </summary>
    /// <param name="source">The data source to execute the query on.</param>
    /// <returns>The query result of type <typeparamref name="TResult"/>.</returns>
    IQueryable<TResult> Execute(IQuerySource source);
}
