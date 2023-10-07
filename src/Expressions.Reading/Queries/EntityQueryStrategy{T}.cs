namespace Raiqub.Expressions.Queries;

/// <summary>
/// Represents a base implementation of the <see cref="IEntityQueryStrategy{TSource}"/> interface that provides
/// a mechanism for defining and executing queries.
/// </summary>
/// <typeparam name="TSource">The type of the data source and the result for the query strategy.</typeparam>
public abstract class EntityQueryStrategy<TSource>
    : EntityQueryStrategy<TSource, TSource>, IEntityQueryStrategy<TSource>
    where TSource : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EntityQueryStrategy{TSource}"/> class with no restrictions.
    /// </summary>
    protected EntityQueryStrategy()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityQueryStrategy{TSource}"/> class with the specified restrictions.
    /// </summary>
    /// <param name="restrictions">The restrictions to apply to the query.</param>
    protected EntityQueryStrategy(params Specification<TSource>[] restrictions) : base(restrictions)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityQueryStrategy{TSource}"/> class with the specified restrictions.
    /// </summary>
    /// <param name="restrictions">The restrictions to apply to the query.</param>
    protected EntityQueryStrategy(IEnumerable<Specification<TSource>> restrictions) : base(restrictions)
    {
    }

    /// <summary>
    /// Executes the query strategy on the specified data source and returns the query result as an <see cref="IQueryable{T}"/>.
    /// </summary>
    /// <param name="source">The data source to execute the query.</param>
    /// <returns>An <see cref="IQueryable{T}"/> that represents the query result.</returns>
    protected override IQueryable<TSource> ExecuteCore(IQueryable<TSource> source) => source;
}
