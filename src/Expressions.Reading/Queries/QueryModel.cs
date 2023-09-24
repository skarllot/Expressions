using System.Linq.Expressions;
using Raiqub.Expressions.Queries.Internal;

namespace Raiqub.Expressions.Queries;

public static class QueryModel
{
    /// <summary>Returns a query model for the specified type <typeparamref name="TSource"/> that returns all items.</summary>
    /// <typeparam name="TSource">The type of the data source.</typeparam>
    /// <returns>A query model for the specified type <typeparamref name="TSource"/>.</returns>
    public static IEntityQueryModel<TSource> AllOfEntity<TSource>() where TSource : class =>
        AllQueryModel<TSource>.Instance;

    /// <summary>Creates a new query strategy from the specified function.</summary>
    /// <param name="queryModel">A function defining the query strategy.</param>
    /// <typeparam name="TResult">The type of the query result.</typeparam>
    /// <returns>A new query strategy.</returns>
    public static IQueryModel<TResult> Create<TResult>(Func<IQuerySource, IQueryable<TResult>> queryModel) =>
        new AnonymousQueryModel<TResult>(queryModel);

    /// <summary>
    /// Creates a new entity query strategy that returns items from data source of type <typeparamref name="TSource"/>.
    /// It only returns the items that satisfies the specified business rule.
    /// </summary>
    /// <param name="specification"></param>
    /// <typeparam name="TSource">The type of the data source.</typeparam>
    /// <returns>A new entity query strategy.</returns>
    public static IEntityQueryModel<TSource> CreateForEntity<TSource>(Specification<TSource> specification) where TSource : class =>
        new SpecificationQueryModel<TSource>(specification);

    /// <summary>Creates a new entity query strategy from the specified function.</summary>
    /// <param name="queryModel">A function defining the query strategy.</param>
    /// <typeparam name="TSource">The type of the data source.</typeparam>
    /// <typeparam name="TResult">The type of the query result.</typeparam>
    /// <returns>A new entity query strategy.</returns>
    public static IEntityQueryModel<TSource, TResult> CreateForEntity<TSource, TResult>(
        Func<IQueryable<TSource>, IQueryable<TResult>> queryModel) where TSource : class =>
        new AnonymousEntityQueryModel<TSource, TResult>(queryModel);

    /// <summary>
    /// Creates a new query model that returns all items for the nested collection of the specified entity.
    /// </summary>
    /// <param name="selector">A projection function to retrieve nested collection.</param>
    /// <typeparam name="TSource">The type of entity to query.</typeparam>
    /// <typeparam name="TNested">The type of nested collection from the specified entity.</typeparam>
    /// <returns>A new query model.</returns>
    public static IEntityQueryModel<TSource, TNested> CreateNested<TSource, TNested>(
        Expression<Func<TSource, IEnumerable<TNested>>> selector)
        where TSource : class
        where TNested : class =>
        new NestingEntityQueryModel<TSource, TNested, TNested>(selector, AllOfEntity<TNested>());

    /// <summary>
    /// Creates a new query model that returns the items of the nested collection of the specified entity.
    /// It only returns the nested items that satisfies the specified business rule.
    /// </summary>
    /// <param name="selector">A projection function to retrieve nested collection.</param>
    /// <param name="specification">The specification used for query.</param>
    /// <typeparam name="TSource">The type of entity to query.</typeparam>
    /// <typeparam name="TNested">The type of nested collection from the specified entity.</typeparam>
    /// <returns>A new query model.</returns>
    public static IEntityQueryModel<TSource, TNested> CreateNested<TSource, TNested>(
        Expression<Func<TSource, IEnumerable<TNested>>> selector,
        Specification<TNested> specification)
        where TSource : class
        where TNested : class =>
        new NestingEntityQueryModel<TSource, TNested, TNested>(selector, CreateForEntity(specification));

    /// <summary>
    /// Creates a new query model for the nested collection of the specified entity.
    /// The nested items are then queried using the specified query model.
    /// </summary>
    /// <param name="selector">A projection function to retrieve nested collection.</param>
    /// <param name="queryModel">The query model to use with nested elements.</param>
    /// <typeparam name="TSource">The type of entity to query.</typeparam>
    /// <typeparam name="TNested">The type of nested collection from the specified entity.</typeparam>
    /// <typeparam name="TResult">The type of result to return.</typeparam>
    /// <returns>A new query model.</returns>
    public static IEntityQueryModel<TSource, TResult> CreateNested<TSource, TNested, TResult>(
        Expression<Func<TSource, IEnumerable<TNested>>> selector,
        IEntityQueryModel<TNested, TResult> queryModel)
        where TSource : class
        where TNested : class =>
        new NestingEntityQueryModel<TSource, TNested, TResult>(selector, queryModel);
}
