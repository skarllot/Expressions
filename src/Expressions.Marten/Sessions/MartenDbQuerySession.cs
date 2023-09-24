using Marten;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Marten.Queries;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Expressions.Marten.Sessions;

public class MartenDbQuerySession : IDbQuerySession
{
    private readonly ILogger<MartenDbQuerySession> _logger;
    private readonly IQuerySession _session;
    private readonly MartenQuerySource _querySource;

    public MartenDbQuerySession(ILogger<MartenDbQuerySession> logger, IQuerySession session)
    {
        _logger = logger;
        _session = session;
        _querySource = new MartenQuerySource(session);
    }

    public IDbQuery<TResult> Query<TResult>(IQueryStrategy<TResult> queryStrategy)
    {
        return new MartenDbQuery<TResult>(_logger, queryStrategy.Execute(_querySource));
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        await _session.DisposeAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _session.Dispose();
        }
    }
}
