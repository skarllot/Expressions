using Microsoft.EntityFrameworkCore;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Expressions.EntityFrameworkCore.Sessions;

public class DbSession<TContext> : ISession<TContext>
    where TContext : DbContext
{
    public DbSession(TContext context, ChangeTracking? tracking)
    {
        Context = context;
        Tracking = tracking;
    }

    public TContext Context { get; }
    public ChangeTracking? Tracking { get; }

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

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Context.SaveChangesAsync(true, cancellationToken);
    }

    protected virtual ValueTask DisposeAsyncCore()
    {
        // Nothing to clean
        return new ValueTask();
    }

    protected virtual void Dispose(bool disposing)
    {
        // Nothing to clean
    }
}
