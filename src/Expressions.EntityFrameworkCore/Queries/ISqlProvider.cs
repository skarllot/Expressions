namespace Raiqub.Expressions.EntityFrameworkCore.Queries;

/// <summary>Defines a contract for a SQL provider that generates SQL strings for a specific entity type.</summary>
public interface ISqlProvider
{
    /// <summary>Gets the type of entity for which SQL queries are generated.</summary>
    Type EntityType { get; }

    /// <summary>Gets the SQL string for querying entity from a database.</summary>
    /// <returns>The SQL query as a <see cref="SqlString"/> instance.</returns>
    /// <remarks>
    /// Implementations of this method should return a SQL query string tailored to retrieve
    /// data related to the specified entity. The returned value can be cached.
    /// </remarks>
    SqlString GetQuerySql();
}

/// <summary>Defines a contract for a SQL provider that generates SQL strings for a specific entity type.</summary>
/// <typeparam name="TEntity">The type of entity for which SQL queries are generated.</typeparam>
public interface ISqlProvider<TEntity> : ISqlProvider
    where TEntity : class
{
    /// <inheritdoc />
    Type ISqlProvider.EntityType => typeof(TEntity);
}
