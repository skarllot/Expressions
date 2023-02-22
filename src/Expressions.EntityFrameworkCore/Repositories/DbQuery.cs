using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Repositories;

namespace Raiqub.Expressions.EntityFrameworkCore.Repositories;

public class DbQuery<TSource, TResult> : IQuery<TResult>
    where TSource : class
{
    private readonly ILogger _logger;
    private readonly DbContext _dbContext;
    private readonly QueryModel<TSource, TResult> _queryModel;
    private readonly ChangeTracking _tracking;

    public DbQuery(
        ILogger logger,
        DbContext dbContext,
        QueryModel<TSource, TResult> queryModel,
        ChangeTracking tracking)
    {
        _logger = logger;
        _dbContext = dbContext;
        _queryModel = queryModel;
        _tracking = tracking;
    }

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await GetDbSet()
                .Query(_queryModel)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _logger.LogError(exception, "Error trying to query whether any element exists");
            throw;
        }
    }

    public async Task<long> CountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await GetDbSet()
                .Query(_queryModel)
                .LongCountAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _logger.LogError(exception, "Error trying to count elements");
            throw;
        }
    }

    public async Task<TResult?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await GetDbSet(_tracking)
                .Query(_queryModel)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _logger.LogError(exception, "Error trying to query the first element");
            throw;
        }
    }

    public async Task<List<TResult>> ListAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await GetDbSet(_tracking)
                .Query(_queryModel)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _logger.LogError(exception, "Error trying to list found elements");
            throw;
        }
    }

    public async Task<TResult?> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await GetDbSet(_tracking)
                .Query(_queryModel)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _logger.LogError(exception, "Error trying to query the single element");
            throw;
        }
    }

    protected IQueryable<TSource> GetDbSet(ChangeTracking tracking = ChangeTracking.Default)
    {
        IQueryable<TSource> queryable = _dbContext.Set<TSource>();

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
