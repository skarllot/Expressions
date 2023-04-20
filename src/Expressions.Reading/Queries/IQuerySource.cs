namespace Raiqub.Expressions.Queries;

/// <summary>Represents a provider of data sources.</summary>
public interface IQuerySource
{
    /// <summary>Gets the data source of the entity of type <typeparamref name="TEntity"/>.</summary>
    /// <typeparam name="TEntity">The type of entity to query.</typeparam>
    /// <returns>The data source of type <typeparamref name="TEntity"/>.</returns>
    IQueryable<TEntity> GetSet<TEntity>() where TEntity : class;
}
