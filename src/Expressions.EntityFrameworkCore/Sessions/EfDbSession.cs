using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.EntityFrameworkCore.Options;
using Raiqub.Expressions.EntityFrameworkCore.Queries;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Expressions.EntityFrameworkCore.Sessions;

public class EfDbSession : IDbSession
{
    private readonly ILogger<EfDbSession> _logger;
    private readonly EfQuerySource _querySource;

    public EfDbSession(
        ILogger<EfDbSession> logger,
        DbContext dbContext,
        ISqlProviderSelector sqlProviderSelector,
        EntityOptionsSelector optionsSelector,
        ChangeTracking tracking)
    {
        _logger = logger;
        DbContext = dbContext;
        Tracking = tracking;
        _querySource = new EfQuerySource(dbContext, sqlProviderSelector, optionsSelector, tracking);
    }

    public DbContext DbContext { get; }
    public ChangeTracking Tracking { get; }

    public async ValueTask<IDbSessionTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await DbContext.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        return new EfDbSessionTransaction(transaction);
    }

    public void Add<TEntity>(TEntity entity)
        where TEntity : class => DbContext.Add(entity);

    public void AddRange<TEntity>(IEnumerable<TEntity> entities)
        where TEntity : class => DbContext.AddRange(entities);

    public async ValueTask AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class
    {
        await DbContext
            .AddAsync(entity, cancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask AddRangeAsync<TEntity>(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        await DbContext
            .AddRangeAsync(entities, cancellationToken)
            .ConfigureAwait(false);
    }

    public IDbQuery<TResult> Query<TResult>(IQueryStrategy<TResult> queryStrategy)
    {
        return new EfDbQuery<TResult>(
            _logger,
            queryStrategy.Execute(_querySource));
    }

    public void Remove<TEntity>(TEntity entity) where TEntity : class => DbContext.Remove(entity);

    public void RemoveRange<TEntity>(IEnumerable<TEntity> entities)
        where TEntity : class => DbContext.RemoveRange(entities);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        DbContext.SaveChangesAsync(true, cancellationToken);

    public void Update<TEntity>(TEntity entity)
        where TEntity : class => DbContext.Update(entity);

    public void UpdateRange<TEntity>(IEnumerable<TEntity> entities)
        where TEntity : class => DbContext.UpdateRange(entities);

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
        await DbContext.DisposeAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            DbContext.Dispose();
        }
    }
}
