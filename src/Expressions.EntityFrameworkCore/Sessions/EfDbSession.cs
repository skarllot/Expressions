using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.EntityFrameworkCore.Queries;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Sessions;
using Raiqub.Expressions.Sessions.BoundedContext;

namespace Raiqub.Expressions.EntityFrameworkCore.Sessions;

public class EfDbSession<TContext> : IDbSession<TContext>
    where TContext : DbContext
{
    private readonly ILogger<EfDbSession<TContext>> _logger;
    private readonly EfQuerySource _querySource;

    public EfDbSession(
        ILogger<EfDbSession<TContext>> logger,
        TContext context,
        ISqlProviderSelector sqlProviderSelector,
        ChangeTracking tracking)
    {
        _logger = logger;
        Context = context;
        Tracking = tracking;
        _querySource = new EfQuerySource(context, sqlProviderSelector, tracking);
    }

    public TContext Context { get; }
    public ChangeTracking Tracking { get; }

    public async ValueTask<IDbSessionTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await Context.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        return new EfDbSessionTransaction(transaction);
    }

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

    public IDbQuery<TResult> Query<TResult>(IQueryStrategy<TResult> queryStrategy)
    {
        return new EfDbQuery<TResult>(
            _logger,
            queryStrategy.Execute(_querySource));
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

    protected virtual async ValueTask DisposeAsyncCore()
    {
        await Context.DisposeAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Context.Dispose();
        }
    }
}
