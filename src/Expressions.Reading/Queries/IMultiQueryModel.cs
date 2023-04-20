﻿namespace Raiqub.Expressions.Queries;

/// <summary>
/// Represents a query model that can execute a query aggregating data sources of multiple entities
/// and return a result of type <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TResult">The type of the query result.</typeparam>
public interface IMultiQueryModel<out TResult>
{
    /// <summary>
    /// Executes the query using the specified query source and
    /// returns a query result of type <typeparamref name="TResult"/>.
    /// </summary>
    /// <param name="source">The data source to execute the query on.</param>
    /// <returns>The query result of type <typeparamref name="TResult"/>.</returns>
    IQueryable<TResult> Execute(IQuerySource source);
}
