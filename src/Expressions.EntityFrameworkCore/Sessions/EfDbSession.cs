using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.EntityFrameworkCore.Options;
using Raiqub.Expressions.EntityFrameworkCore.Queries;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Expressions.EntityFrameworkCore.Sessions;

/// <summary>Entity Framework-based implementation of a database session for querying and saving instances.</summary>
public class EfDbSession : IDbSession
{
    private readonly ILogger<EfDbSession> _logger;
    private readonly EfQuerySource _querySource;

    /// <summary>Initializes a new instance of the <see cref="EfDbSession"/> class.</summary>
    /// <param name="logger">The <see cref="ILogger"/> to log to.</param>
    /// <param name="dbContext">The <see cref="Microsoft.EntityFrameworkCore.DbContext"/> to read/write.</param>
    /// <param name="sqlProviderSelector">A selector to retrieve custom SQL for querying entities.</param>
    /// <param name="optionsSelector">A selector to retrieve options for handling entities.</param>
    /// <param name="tracking">The change tracking mode of the session.</param>
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

    /// <summary>Gets the Entity Framework session of this database session.</summary>
    public DbContext DbContext { get; }

    /// <inheritdoc />
    public ChangeTracking Tracking { get; }

    /// <inheritdoc />
    public async ValueTask<IDbSessionTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var transaction = await DbContext.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        return new EfDbSessionTransaction(transaction);
    }

    /// <inheritdoc />
    public void Add<TEntity>(TEntity entity)
        where TEntity : class => DbContext.Add(entity);

    /// <inheritdoc />
    public void AddRange<TEntity>(IEnumerable<TEntity> entities)
        where TEntity : class => DbContext.AddRange(entities);

    /// <inheritdoc />
    public async ValueTask AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class
    {
        await DbContext
            .AddAsync(entity, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask AddRangeAsync<TEntity>(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        await DbContext
            .AddRangeAsync(entities, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public IDbQuery<TEntity> Query<TEntity>() where TEntity : class
    {
        return new EfDbQuery<TEntity>(_logger, DbQueryScope.Create<TEntity>(), _querySource.GetSet<TEntity>());
    }

    /// <inheritdoc />
    public IDbQuery<TEntity> Query<TEntity>(Specification<TEntity> specification) where TEntity : class
    {
        return new EfDbQuery<TEntity>(
            _logger,
            DbQueryScope.Create(specification),
            _querySource.GetSet<TEntity>().Where(specification));
    }

    /// <inheritdoc />
    public IDbQuery<TResult> Query<TEntity, TResult>(IEntityQueryStrategy<TEntity, TResult> queryStrategy)
        where TEntity : class
        where TResult : notnull
    {
        return new EfDbQuery<TResult>(
            _logger,
            DbQueryScope.Create(queryStrategy),
            queryStrategy.Execute(_querySource.GetSet<TEntity>()));
    }

    /// <inheritdoc />
    public IDbQuery<TResult> Query<TResult>(IQueryStrategy<TResult> queryStrategy)
        where TResult : notnull
    {
        return new EfDbQuery<TResult>(_logger, DbQueryScope.Create(queryStrategy), queryStrategy.Execute(_querySource));
    }

    /// <inheritdoc />
    public IDbQueryValue<TResult> QueryValue<TEntity, TResult>(IEntityQueryStrategy<TEntity, TResult> queryStrategy)
        where TEntity : class
        where TResult : struct
    {
        return new EfDbQueryValue<TResult>(
            _logger,
            DbQueryScope.Create(queryStrategy),
            queryStrategy.Execute(_querySource.GetSet<TEntity>()));
    }

    /// <inheritdoc />
    public IDbQueryValue<TResult> QueryValue<TResult>(IQueryStrategy<TResult> queryStrategy)
        where TResult : struct
    {
        return new EfDbQueryValue<TResult>(
            _logger,
            DbQueryScope.Create(queryStrategy),
            queryStrategy.Execute(_querySource));
    }

    /// <inheritdoc />
    public void Remove<TEntity>(TEntity entity) where TEntity : class => DbContext.Remove(entity);

    /// <inheritdoc />
    public void RemoveRange<TEntity>(IEnumerable<TEntity> entities)
        where TEntity : class => DbContext.RemoveRange(entities);

    /// <inheritdoc />
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        DbContext.SaveChangesAsync(true, cancellationToken);

    /// <inheritdoc />
    public void Update<TEntity>(TEntity entity)
        where TEntity : class => DbContext.Update(entity);

    /// <inheritdoc />
    public void UpdateRange<TEntity>(IEnumerable<TEntity> entities)
        where TEntity : class => DbContext.UpdateRange(entities);

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>DisposeAsync method for implementations to write.</summary>
    protected virtual async ValueTask DisposeAsyncCore()
    {
        await DbContext.DisposeAsync();
    }

    /// <summary>Dispose method for implementations to write.</summary>
    /// <param name="disposing"><see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            DbContext.Dispose();
        }
    }
}
