namespace Raiqub.Expressions.Queries;

/// <summary>Represents a provider of data sources.</summary>
public interface IQuerySource
{
    /// <summary>Gets the data source of the entity of type <typeparamref name="TEntity"/>.</summary>
    /// <typeparam name="TEntity">The type of entity to query.</typeparam>
    /// <returns>The data source of type <typeparamref name="TEntity"/>.</returns>
    IQueryable<TEntity> GetSet<TEntity>() where TEntity : class;

    /// <summary>Creates a LINQ query based on a SQL query.</summary>
    /// <typeparam name="TEntity">The type of entity to query.</typeparam>
    /// <param name="sql">The interpolated string representing a SQL query with parameters.</param>
    /// <returns>The data source of type <typeparamref name="TEntity"/>.</returns>
    IQueryable<TEntity> GetSetFromSql<TEntity>(FormattableString sql) where TEntity : class;

    /// <summary>Creates a LINQ query based on a SQL query.</summary>
    /// <typeparam name="TEntity">The type of entity to query.</typeparam>
    /// <param name="sql">The raw SQL query.</param>
    /// <param name="parameters">The values to be assigned to parameters.</param>
    /// <returns>The data source of type <typeparamref name="TEntity"/>.</returns>
    IQueryable<TEntity> GetSetFromRawSql<TEntity>(string sql, params object[] parameters) where TEntity : class;

    /// <summary>Creates a LINQ query based on a SQL query for a non-mapped type.</summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="sql">The interpolated string representing a SQL query with parameters.</param>
    /// <returns>An <see cref="IQueryable{TResult}"/> representing the interpolated string SQL query.</returns>
    IQueryable<TResult> GetNonMappedFromSql<TResult>(FormattableString sql);

    /// <summary>Creates a LINQ query based on a raw SQL query for a non-mapped type.</summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="sql">The raw SQL query.</param>
    /// <param name="parameters">The values to be assigned to parameters.</param>
    /// <returns>An <see cref="IQueryable{TResult}"/> representing the raw SQL query.</returns>
    IQueryable<TResult> GetNonMappedFromRawSql<TResult>(string sql, params object[] parameters);
}
