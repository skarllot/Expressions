namespace Raiqub.Expressions.EntityFrameworkCore.Queries;

/// <summary>
/// Defines a contract for a selector that retrieves SQL queries based on a specified entity type.
/// </summary>
[Obsolete("Use the method GetSetFromSql from IQuerySource instead")]
public interface ISqlProviderSelector
{
    /// <summary>
    /// Gets the SQL query represented as a <see cref="SqlString"/> for a specific entity type.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity for which the SQL query is retrieved.</typeparam>
    /// <returns>
    /// The SQL query as a <see cref="SqlString"/> instance, or <c>null</c> if a custom query is not defined.
    /// </returns>
    SqlString? GetQuerySql<TEntity>() where TEntity : class;
}
