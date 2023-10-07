namespace Raiqub.Expressions.Queries;

/// <summary>
/// Represents a base implementation of the <see cref="IEntityQueryStrategy{TSource,TResult}"/> interface that provides
/// a mechanism for defining and executing queries.
/// </summary>
/// <typeparam name="TSource">The type of the data source for the query strategy.</typeparam>
/// <typeparam name="TResult">The type of the query result.</typeparam>
public abstract class EntityQueryStrategy<TSource, TResult>
    : IEntityQueryStrategy<TSource, TResult>
    where TSource : class
{
    private readonly Specification<TSource> _restrictions;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityQueryStrategy{TSource,TResult}"/> class with no restrictions.
    /// </summary>
    protected EntityQueryStrategy()
    {
        _restrictions = Specification.All<TSource>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityQueryStrategy{TSource,TResult}"/> class with the specified restrictions.
    /// </summary>
    /// <param name="restrictions">The restrictions to apply to the query.</param>
    protected EntityQueryStrategy(params Specification<TSource>[] restrictions)
        : this((IEnumerable<Specification<TSource>>)restrictions)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityQueryStrategy{TSource,TResult}"/> class with the specified restrictions.
    /// </summary>
    /// <param name="restrictions">The restrictions to apply to the query.</param>
    protected EntityQueryStrategy(IEnumerable<Specification<TSource>> restrictions)
    {
        _restrictions = Specification.And(restrictions);
    }

    private Specification<TSource> CombinedSpecification =>
        _restrictions.And(Specification.And(GetPreconditions()));

    /// <inheritdoc />
    public IQueryable<TResult> Execute(IQueryable<TSource> source)
    {
        return ExecuteCore(source.Where(CombinedSpecification));
    }

    /// <inheritdoc />
    public IEnumerable<TResult> Execute(IEnumerable<TSource> source)
    {
        return Execute(source.AsQueryable());
    }

    /// <inheritdoc />
    public IQueryable<TResult> Execute(IQuerySource source)
    {
        return Execute(source.GetSet<TSource>());
    }

    /// <summary>Gets the mandatory preconditions to execute the query.</summary>
    /// <returns>An <see cref="IEnumerable{T}"/> that represents the preconditions for the query strategy.</returns>
    protected virtual IEnumerable<Specification<TSource>> GetPreconditions() =>
        Enumerable.Empty<Specification<TSource>>();

    /// <summary>
    /// Executes the query strategy on the specified data source and returns the query result as an <see cref="IQueryable{T}"/>.
    /// </summary>
    /// <param name="source">The data source to execute the query.</param>
    /// <returns>An <see cref="IQueryable{T}"/> that represents the query result.</returns>
    protected abstract IQueryable<TResult> ExecuteCore(IQueryable<TSource> source);
}
