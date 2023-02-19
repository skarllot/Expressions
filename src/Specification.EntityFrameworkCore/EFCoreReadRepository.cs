using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Raiqub.Specification.EntityFrameworkCore;

public class EFCoreReadRepository<TDbContext, TEntity> : IReadRepository<TEntity>
    where TDbContext : DbContext
    where TEntity : class
{
    private readonly ILogger<EFCoreReadRepository<TDbContext, TEntity>> _logger;
    private readonly DbContext _dbContext;

    public EFCoreReadRepository(ILogger<EFCoreReadRepository<TDbContext, TEntity>> logger, TDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public Task<bool> AnyAsync(
        CancellationToken cancellationToken = default) =>
        AnyAsync(Query.Create<TEntity>(), cancellationToken);

    public Task<bool> AnyAsync(
        Specification<TEntity> specification,
        CancellationToken cancellationToken = default) =>
        AnyAsync(Query.Create(specification), cancellationToken);

    public async Task<bool> AnyAsync<TResult>(
        Query<TEntity, TResult> query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await GetDbSet()
                .Query(query)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _logger.LogError(exception, "Error trying to query whether any element exists");
            throw;
        }
    }

    public Task<long> CountAsync(
        CancellationToken cancellationToken = default) =>
        CountAsync(Query.Create<TEntity>(), cancellationToken);

    public Task<long> CountAsync(
        Specification<TEntity> specification,
        CancellationToken cancellationToken = default) =>
        CountAsync(Query.Create(specification), cancellationToken);

    public async Task<long> CountAsync<TResult>(
        Query<TEntity, TResult> query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await GetDbSet()
                .Query(query)
                .LongCountAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _logger.LogError(exception, "Error trying to count elements");
            throw;
        }
    }

    public Task<TEntity?> FirstOrDefaultAsync(
        CancellationToken cancellationToken) =>
        FirstOrDefaultAsync(Query.Create<TEntity>(), ChangeTracking.Default, cancellationToken);

    public Task<TEntity?> FirstOrDefaultAsync(
        ChangeTracking tracking = ChangeTracking.Default,
        CancellationToken cancellationToken = default) =>
        FirstOrDefaultAsync(Query.Create<TEntity>(), tracking, cancellationToken);

    public Task<TEntity?> FirstOrDefaultAsync(
        Specification<TEntity> specification,
        CancellationToken cancellationToken) =>
        FirstOrDefaultAsync(Query.Create(specification), ChangeTracking.Default, cancellationToken);

    public Task<TEntity?> FirstOrDefaultAsync(
        Specification<TEntity> specification,
        ChangeTracking tracking = ChangeTracking.Default,
        CancellationToken cancellationToken = default) =>
        FirstOrDefaultAsync(Query.Create(specification), tracking, cancellationToken);

    public Task<TResult?> FirstOrDefaultAsync<TResult>(
        Query<TEntity, TResult> query,
        CancellationToken cancellationToken) =>
        FirstOrDefaultAsync(query, query.DefaultChangeTracking, cancellationToken);

    public async Task<TResult?> FirstOrDefaultAsync<TResult>(
        Query<TEntity, TResult> query,
        ChangeTracking tracking = ChangeTracking.Default,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await GetDbSet(tracking)
                .Query(query)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _logger.LogError(exception, "Error trying to query the first element");
            throw;
        }
    }

    public Task<List<TEntity>> ListAsync(
        CancellationToken cancellationToken) =>
        ListAsync(Query.Create<TEntity>(), ChangeTracking.Default, cancellationToken);

    public Task<List<TEntity>> ListAsync(
        ChangeTracking tracking = ChangeTracking.Default,
        CancellationToken cancellationToken = default) =>
        ListAsync(Query.Create<TEntity>(), tracking, cancellationToken);

    public Task<List<TEntity>> ListAsync(
        Specification<TEntity> specification,
        CancellationToken cancellationToken) =>
        ListAsync(Query.Create(specification), ChangeTracking.Default, cancellationToken);

    public Task<List<TEntity>> ListAsync(
        Specification<TEntity> specification,
        ChangeTracking tracking = ChangeTracking.Default,
        CancellationToken cancellationToken = default) =>
        ListAsync(Query.Create(specification), tracking, cancellationToken);

    public Task<List<TResult>> ListAsync<TResult>(
        Query<TEntity, TResult> query,
        CancellationToken cancellationToken) =>
        ListAsync(query, query.DefaultChangeTracking, cancellationToken);

    public async Task<List<TResult>> ListAsync<TResult>(
        Query<TEntity, TResult> query,
        ChangeTracking tracking = ChangeTracking.Default,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await GetDbSet(tracking)
                .Query(query)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _logger.LogError(exception, "Error trying to list found elements");
            throw;
        }
    }

    public Task<TEntity?> SingleOrDefaultAsync(
        CancellationToken cancellationToken) =>
        SingleOrDefaultAsync(Query.Create<TEntity>(), ChangeTracking.Default, cancellationToken);

    public Task<TEntity?> SingleOrDefaultAsync(
        ChangeTracking tracking = ChangeTracking.Default,
        CancellationToken cancellationToken = default) =>
        SingleOrDefaultAsync(Query.Create<TEntity>(), tracking, cancellationToken);

    public Task<TEntity?> SingleOrDefaultAsync(
        Specification<TEntity> specification,
        CancellationToken cancellationToken) =>
        SingleOrDefaultAsync(Query.Create(specification), ChangeTracking.Default, cancellationToken);

    public Task<TEntity?> SingleOrDefaultAsync(
        Specification<TEntity> specification,
        ChangeTracking tracking = ChangeTracking.Default,
        CancellationToken cancellationToken = default) =>
        SingleOrDefaultAsync(Query.Create(specification), tracking, cancellationToken);

    public Task<TResult?> SingleOrDefaultAsync<TResult>(
        Query<TEntity, TResult> query,
        CancellationToken cancellationToken) =>
        SingleOrDefaultAsync(query, query.DefaultChangeTracking, cancellationToken);

    public async Task<TResult?> SingleOrDefaultAsync<TResult>(
        Query<TEntity, TResult> query,
        ChangeTracking tracking = ChangeTracking.Default,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await GetDbSet(tracking)
                .Query(query)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _logger.LogError(exception, "Error trying to query the single element");
            throw;
        }
    }

    protected IQueryable<TEntity> GetDbSet(ChangeTracking tracking = ChangeTracking.Default)
    {
        IQueryable<TEntity> queryable = _dbContext.Set<TEntity>();

        queryable = tracking switch
        {
            ChangeTracking.Default => queryable,
            ChangeTracking.Enable => queryable.AsTracking(),
            ChangeTracking.Disable => queryable.AsNoTracking(),
            ChangeTracking.IdentityResolution => queryable.AsNoTrackingWithIdentityResolution(),
            _ => throw new ArgumentException(
                $"The specified change tracking mode is not supported: {tracking}",
                nameof(tracking))
        };

        return queryable;
    }
}
