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

    public void Add<TEntity>(TEntity entity)
        where TEntity : class => Context.Add(entity);

    public void AddRange<TEntity>(IEnumerable<TEntity> entities)
        where TEntity : class => Context.AddRange(entities);

    public async ValueTask AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class
    {
        await Context
            .AddAsync(entity, cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask AddRangeAsync<TEntity>(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        await Context
            .AddRangeAsync(entities, cancellationToken)
            .ConfigureAwait(false);
    }

    public IQuery<TResult> Query<TEntity, TResult>(IQueryModel<TEntity, TResult> queryModel)
        where TEntity : class
    {
        return new DbQuery<TEntity, TResult>(
            _logger,
            Context,
            queryModel,
            Tracking);
    }

    public void Remove<TEntity>(TEntity entity) where TEntity : class => Context.Remove(entity);

    public void RemoveRange<TEntity>(IEnumerable<TEntity> entities)
        where TEntity : class => Context.RemoveRange(entities);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        Context.SaveChangesAsync(true, cancellationToken);

    public void Update<TEntity>(TEntity entity)
        where TEntity : class => Context.Update(entity);

    public void UpdateRange<TEntity>(IEnumerable<TEntity> entities)
        where TEntity : class => Context.UpdateRange(entities);

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
        // Nothing to clean
        return new ValueTask();
    }

    protected virtual void Dispose(bool disposing)
    {
        // Nothing to clean
    }
}
