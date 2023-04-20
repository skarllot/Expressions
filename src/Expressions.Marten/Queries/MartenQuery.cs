using Marten;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.Marten.Queries;

public class MartenQuery<TResult> : IQuery<TResult>
{
    private readonly ILogger _logger;
    private readonly IQueryable<TResult> _dataSource;

    public MartenQuery(
        ILogger logger,
        IQueryable<TResult> dataSource)
    {
        _logger = logger;
        _dataSource = dataSource;
    }

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dataSource
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
            return await _dataSource
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
            return await _dataSource
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
            return await _dataSource
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
            return await _dataSource
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            QueryLog.SingleError(_logger, exception);
            throw;
        }
    }
}
