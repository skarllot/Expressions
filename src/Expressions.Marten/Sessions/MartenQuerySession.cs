﻿using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Marten.Queries;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Expressions.Marten.Sessions;

public class MartenQuerySession : IQuerySession
{
    private readonly ILogger<MartenQuerySession> _logger;
    private readonly global::Marten.IQuerySession _session;

    public MartenQuerySession(ILogger<MartenQuerySession> logger, global::Marten.IQuerySession session)
    {
        _logger = logger;
        _session = session;
    }

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