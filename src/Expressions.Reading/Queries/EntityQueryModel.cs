namespace Raiqub.Expressions.Queries;

[Obsolete("Use QueryModel static class instead", true)]
public static class EntityQueryModel
{
    /// <summary>
    /// Returns a query model for the specified type <typeparamref name="T"/> that does nothing.
    /// </summary>
    /// <typeparam name="T">The type of the elements of query model.</typeparam>
    /// <returns>A query model for the specified type <typeparamref name="T"/>.</returns>
    public static IEntityQueryModel<T> Create<T>() where T : class =>
        QueryModel.AllOfEntity<T>();

    public static IEntityQueryModel<T> Create<T>(Specification<T> specification) where T : class =>
        QueryModel.CreateForEntity(specification);

    public static IEntityQueryModel<TSource, TResult> Create<TSource, TResult>(
        Func<IQueryable<TSource>, IQueryable<TResult>> queryModel) where TSource : class =>
        QueryModel.CreateForEntity(queryModel);
}
