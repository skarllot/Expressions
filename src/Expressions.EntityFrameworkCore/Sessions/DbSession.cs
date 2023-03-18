using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.EntityFrameworkCore.Queries;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Sessions;
using Raiqub.Expressions.Sessions.BoundedContext;

namespace Raiqub.Expressions.EntityFrameworkCore.Sessions;

public class DbSession<TContext> : ISession<TContext>, ISession
    where TContext : DbContext
{
    private readonly ILogger<DbSession<TContext>> _logger;

    public DbSession(ILogger<DbSession<TContext>> logger, TContext context, ChangeTracking tracking)
    {
        _logger = logger;
        Context = context;
        Tracking = tracking;
    }

    public TContext Context { get; }
    public ChangeTracking Tracking { get; }

    public IQuery<TResult> Query<TEntity, TResult>(IQueryModel<TEntity, TResult> queryModel)
        where TEntity : class
    {
        return new DbQuery<TEntity, TResult>(
            _logger,
            Context,
            queryModel,
            Tracking);
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
