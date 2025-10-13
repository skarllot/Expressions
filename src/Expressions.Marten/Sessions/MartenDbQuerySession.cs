using Marten;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Marten.Queries;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Expressions.Marten.Sessions;

/// <summary>Marten-based implementation of a database session for querying instances.</summary>
public class MartenDbQuerySession : IDbQuerySession
{
    private readonly ILogger<MartenDbQuerySession> _logger;
    private readonly IQuerySession _session;
    private MartenQuerySource? _querySource;

    /// <summary>Initializes a new instance of the <see cref="MartenDbQuerySession"/> class.</summary>
    /// <param name="logger">The <see cref="ILogger"/> to log to.</param>
    /// <param name="session">The Marten session to query from.</param>
    public MartenDbQuerySession(ILogger<MartenDbQuerySession> logger, IQuerySession session)
    {
        _logger = logger;
        _session = session;
    }

    /// <summary>Gets the Marten session used by this database session.</summary>
    public virtual IQuerySession MartenSession => _session;

    private MartenQuerySource QuerySource => _querySource ??= new MartenQuerySource(_session);

    /// <inheritdoc />
    public IDbQuery<TEntity> Query<TEntity>() where TEntity : class
    {
        return new MartenDbQuery<TEntity>(_logger, _session.Query<TEntity>());
    }

    /// <inheritdoc />
    public IDbQuery<TEntity> Query<TEntity>(Specification<TEntity> specification) where TEntity : class
    {
        return new MartenDbQuery<TEntity>(_logger, _session.Query<TEntity>().Where(specification));
    }

    /// <inheritdoc />
    public IDbQuery<TResult> Query<TEntity, TResult>(IEntityQueryStrategy<TEntity, TResult> queryStrategy)
        where TEntity : class
        where TResult : notnull
    {
        return new MartenDbQuery<TResult>(_logger, queryStrategy.Execute(_session.Query<TEntity>()));
    }

    /// <inheritdoc />
    public IDbQuery<TResult> Query<TResult>(IQueryStrategy<TResult> queryStrategy)
        where TResult : notnull
    {
        return new MartenDbQuery<TResult>(_logger, queryStrategy.Execute(QuerySource));
    }

    /// <inheritdoc />
    public IDbQueryValue<TResult> QueryValue<TEntity, TResult>(IEntityQueryStrategy<TEntity, TResult> queryStrategy)
        where TEntity : class
        where TResult : struct
    {
        return new MartenDbQueryValue<TResult>(_logger, queryStrategy.Execute(_session.Query<TEntity>()));
    }

    /// <inheritdoc />
    public IDbQueryValue<TResult> QueryValue<TResult>(IQueryStrategy<TResult> queryStrategy)
        where TResult : struct
    {
        return new MartenDbQueryValue<TResult>(_logger, queryStrategy.Execute(QuerySource));
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>DisposeAsync method for implementations to write.</summary>
    protected virtual async ValueTask DisposeAsyncCore()
    {
        _querySource = null;
        await _session.DisposeAsync();
    }

    /// <summary>Dispose method for implementations to write.</summary>
    /// <param name="disposing"><see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _querySource = null;
            _session.Dispose();
        }
    }
}
