using Marten;
using Marten.Linq.MatchesSql;
using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.Marten.Queries;

/// <summary>Marten-based implementation of provider of data sources.</summary>
public class MartenQuerySource : IQuerySource
{
    private readonly IQuerySession _session;

    /// <summary>Initializes a new instance of the <see cref="MartenQuerySource"/> class.</summary>
    /// <param name="session">The Marten session to query from.</param>
    public MartenQuerySource(IQuerySession session) => _session = session;

    /// <inheritdoc />
    public IQueryable<TEntity> GetSet<TEntity>() where TEntity : class => _session.Query<TEntity>();

    /// <inheritdoc />
    public IQueryable<TEntity> GetSetFromSql<TEntity>(FormattableString sql) where TEntity : class =>
        throw new NotSupportedException();

    /// <inheritdoc />
    public IQueryable<TEntity> GetSetFromRawSql<TEntity>(string sql, params object[] parameters)
        where TEntity : class => _session.Query<TEntity>().Where(x => x.MatchesSql(sql, parameters));

    /// <inheritdoc />
    public IQueryable<TResult> GetNonMappedFromSql<TResult>(FormattableString sql) =>
        throw new NotSupportedException();

    /// <inheritdoc />
    public IQueryable<TResult> GetNonMappedFromRawSql<TResult>(string sql, params object[] parameters) =>
        throw new NotSupportedException();
}
