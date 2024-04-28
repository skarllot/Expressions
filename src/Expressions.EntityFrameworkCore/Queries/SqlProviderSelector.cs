#pragma warning disable CS0618 // Type or member is obsolete

namespace Raiqub.Expressions.EntityFrameworkCore.Queries;

internal sealed class SqlProviderSelector : ISqlProviderSelector
{
    public static readonly SqlProviderSelector Empty = new(Enumerable.Empty<ISqlProvider>());

    private readonly Dictionary<Type, SqlString> _sqlQueries;

    public SqlProviderSelector(IEnumerable<ISqlProvider> sqlProviders)
    {
        _sqlQueries = sqlProviders.ToDictionary(p => p.EntityType, p => p.GetQuerySql());
    }

    public SqlString? GetQuerySql<TEntity>() where TEntity : class
    {
        return _sqlQueries.TryGetValue(typeof(TEntity), out SqlString value)
            ? value
            : null;
    }
}
