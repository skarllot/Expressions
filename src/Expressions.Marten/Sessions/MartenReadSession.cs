using Marten;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Expressions.Marten.Sessions;

public class MartenReadSession<TContext> : IReadSession<TContext>
    where TContext : IDocumentStore
{
    public MartenReadSession(TContext context)
        : this(context, context.QuerySession())
    {
    }

    protected MartenReadSession(TContext context, IQuerySession session)
    {
        Context = context;
        Session = session;
    }

    public TContext Context { get; }

    public virtual ChangeTracking? Tracking => ChangeTracking.Disable;

    public virtual IQuerySession Session { get; }

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
        return Session.DisposeAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Session.Dispose();
        }
    }
}
