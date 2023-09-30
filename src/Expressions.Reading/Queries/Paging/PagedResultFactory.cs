namespace Raiqub.Expressions.Queries.Paging;

/// <summary>Represents a factory to build paged results.</summary>
/// <typeparam name="TResult">The type of elements returned by the query.</typeparam>
/// <typeparam name="TPage">The type of returned page.</typeparam>
public delegate TPage PagedResultFactory<in TResult, out TPage>(in PageInfo pageInfo, IReadOnlyList<TResult> items);
