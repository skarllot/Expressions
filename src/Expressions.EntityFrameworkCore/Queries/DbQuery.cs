using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Expressions.EntityFrameworkCore.Queries;

public class DbQuery<TSource, TResult> : IQuery<TResult>
    where TSource : class
{
    private readonly ILogger _logger;
    private readonly DbContext _dbContext;
    private readonly IQueryModel<TSource, TResult> _queryModel;
    private readonly ChangeTracking _tracking;

    public DbQuery(
        ILogger logger,
        DbContext dbContext,
        IQueryModel<TSource, TResult> queryModel,
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
                .Apply(_queryModel)
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            QueryLog.AnyError(_logger, exception);
            throw;
        }
    }

    public async Task<long> CountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await GetDbSet()
                .Apply(_queryModel)
                .LongCountAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            QueryLog.CountError(_logger, exception);
            throw;
        }
    }

    public async Task<TResult?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await GetDbSet(_tracking)
                .Apply(_queryModel)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            QueryLog.FirstError(_logger, exception);
            throw;
        }
    }

    public async Task<IReadOnlyList<TResult>> ToListAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await GetDbSet(_tracking)
                .Apply(_queryModel)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            QueryLog.ListError(_logger, exception);
            throw;
        }
    }

    public async Task<TResult?> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await GetDbSet(_tracking)
                .Apply(_queryModel)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            QueryLog.SingleError(_logger, exception);
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
