using Marten;
using Raiqub.Expressions.Repositories;

namespace Raiqub.Expressions.Marten.Repositories;

public class MartenSession : ISession
{
    private readonly IDocumentStore _documentStore;
    private readonly object _gate = new();
    private IDocumentSession? _session;
    private IQuerySession? _querySession;

    public MartenSession(IDocumentStore documentStore)
    {
        _documentStore = documentStore;
    }

    public void Open(ChangeTracking? tracking = null)
    {
        lock (_gate)
        {
            if (_session is not null || _querySession is not null)
                throw new InvalidOperationException("There is a session open already");

            _session = CreateSession(tracking ?? ChangeTracking.Default);
        }
    }

    public void OpenForQuery()
    {
        lock (_gate)
        {
            if (_session is not null || _querySession is not null)
                throw new InvalidOperationException("There is a session open already");

            _querySession = _documentStore.QuerySession();
        }
    }

    public IAsyncDisposable? GetOrOpen(ChangeTracking tracking, out IDocumentSession session)
    {
        IDocumentSession? currentSession;
        bool isQueryOnly;
        lock (_gate)
        {
            currentSession = _session;
            isQueryOnly = _querySession is not null;
        }

        if (isQueryOnly)
        {
            throw new InvalidOperationException("The session is read-only");
        }

        if (currentSession is not null)
        {
            session = currentSession;
            return null;
        }

        session = CreateSession(tracking);
        return session;
    }

    public IAsyncDisposable? GetOrOpenForQuery(out IQuerySession session)
    {
        IQuerySession? currentSession;
        lock (_gate)
        {
            currentSession = _session ?? _querySession;
        }

        if (currentSession is not null)
        {
            session = currentSession;
            return null;
        }

        session = _documentStore.QuerySession();
        return session;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        IDocumentSession? currentSession;
        lock (_gate)
        {
            currentSession = _session;
        }

        return currentSession?.SaveChangesAsync(cancellationToken) ?? Task.CompletedTask;
    }

    private IDocumentSession CreateSession(ChangeTracking tracking = ChangeTracking.Default)
    {
        return tracking switch
        {
            ChangeTracking.Default => _documentStore.OpenSession(),
            ChangeTracking.Enable => _documentStore.OpenSession(DocumentTracking.DirtyTracking),
            ChangeTracking.IdentityResolution => _documentStore.OpenSession(),
            ChangeTracking.Disable => _documentStore.OpenSession(DocumentTracking.None),
            _ => throw new ArgumentException(
                $"The specified change tracking mode is not supported: {tracking}",
                nameof(tracking))
        };
    }
}
