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
    private readonly MartenQuerySource _querySource;

    /// <summary>Initializes a new instance of the <see cref="MartenDbQuerySession"/> class.</summary>
    /// <param name="logger">The <see cref="ILogger"/> to log to.</param>
    /// <param name="session">The Marten session to query from.</param>
    public MartenDbQuerySession(ILogger<MartenDbQuerySession> logger, IQuerySession session)
    {
        _logger = logger;
        _session = session;
        _querySource = new MartenQuerySource(session);
    }

    /// <summary>Gets the Marten session used by this database session.</summary>
    public virtual IQuerySession MartenSession => _session;

    /// <inheritdoc />
    public IDbQuery<TResult> Query<TResult>(IQueryStrategy<TResult> queryStrategy)
    {
        return new MartenDbQuery<TResult>(_logger, queryStrategy.Execute(_querySource));
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
        await _session.DisposeAsync();
    }

    /// <summary>Dispose method for implementations to write.</summary>
    /// <param name="disposing"><see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _session.Dispose();
        }
    }
}
