namespace Raiqub.Expressions.Queries;

/// <summary>
/// Represents a base implementation of the <see cref="IQueryModel{TSource, TResult}"/> interface that provides
/// a mechanism for defining and executing queries.
/// </summary>
/// <typeparam name="TSource">The type of the data source for the query model.</typeparam>
/// <typeparam name="TResult">The type of the query result.</typeparam>
public abstract class QueryModel<TSource, TResult> : IQueryModel<TSource, TResult>
{
    private readonly IEnumerable<Specification<TSource>> _restrictions;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryModel{TSource, TResult}"/> class with no restrictions.
    /// </summary>
    protected QueryModel()
    {
        _restrictions = Enumerable.Empty<Specification<TSource>>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryModel{TSource, TResult}"/> class with the specified restrictions.
    /// </summary>
    /// <param name="restrictions">The restrictions to apply to the query.</param>
    protected QueryModel(params Specification<TSource>[] restrictions)
        : this((IEnumerable<Specification<TSource>>)restrictions)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryModel{TSource, TResult}"/> class with the specified restrictions.
    /// </summary>
    /// <param name="restrictions">The restrictions to apply to the query.</param>
    protected QueryModel(IEnumerable<Specification<TSource>> restrictions)
    {
        _restrictions = restrictions.ToList();
    }

    private Specification<TSource> CombinedSpecification =>
        Specification.And(GetPreconditions().Concat(_restrictions));

    /// <summary>
    /// Executes the query model on the specified data source and returns the query result as an <see cref="IQueryable{T}"/>.
    /// </summary>
    /// <param name="source">The data source to execute the query.</param>
    /// <returns>An <see cref="IQueryable{T}"/> that represents the query result.</returns>
    public IQueryable<TResult> Execute(IQueryable<TSource> source)
    {
        return ExecuteCore(source.Where(CombinedSpecification));
    }

    /// <summary>
    /// Executes the query model on the specified data source and returns the query result as an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <param name="source">The data source to execute the query.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> that represents the query result.</returns>
    public IEnumerable<TResult> Execute(IEnumerable<TSource> source)
    {
        return Execute(source.AsQueryable());
    }

    /// <summary>Gets the mandatory preconditions to execute the query.</summary>
    /// <returns>An <see cref="IEnumerable{T}"/> that represents the preconditions for the query model.</returns>
    protected virtual IEnumerable<Specification<TSource>> GetPreconditions() =>
        Enumerable.Empty<Specification<TSource>>();

    /// <summary>
    /// Executes the query model on the specified data source and returns the query result as an <see cref="IQueryable{T}"/>.
    /// </summary>
    /// <param name="source">The data source to execute the query.</param>
    /// <returns>An <see cref="IQueryable{T}"/> that represents the query result.</returns>
    protected abstract IQueryable<TResult> ExecuteCore(IQueryable<TSource> source);
}
