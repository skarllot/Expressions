using Marten;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Marten.Queries;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Repositories;
using Raiqub.Expressions.Sessions;
using Raiqub.Expressions.Sessions.BoundedContext;
using IQuerySession = Marten.IQuerySession;

namespace Raiqub.Expressions.Marten.Sessions.BoundedContext;

public class MartenQuerySession<TContext> : IQuerySession<TContext>
    where TContext : IDocumentStore
{
    private readonly ILogger<MartenQuerySession<TContext>> _logger;
    private readonly IQuerySession _session;

    public MartenQuerySession(ILogger<MartenQuerySession<TContext>> logger, IQuerySession session, TContext context)
    {
        _logger = logger;
        _session = session;
        Context = context;
    }

    public TContext Context { get; }

    public virtual ChangeTracking Tracking => ChangeTracking.Disable;

    public IQuery<TResult> Query<TEntity, TResult>(IQueryModel<TEntity, TResult> queryModel)
        where TEntity : class
    {
        return new MartenQuery<TEntity, TResult>(
            _logger,
            _session,
            queryModel);
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

    protected virtual ValueTask DisposeAsyncCore()
    {
        return _session.DisposeAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _session.Dispose();
        }
    }
}
