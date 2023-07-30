using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.EntityFrameworkCore.Queries;

public class EFQuery<TResult> : IQuery<TResult>
{
    private readonly ILogger _logger;
    private readonly IQueryable<TResult> _dataSource;

    public EFQuery(
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
        catch (Exception exception) when (exception is not ArgumentNullException
                                              and not OperationCanceledException)
        {
            QueryLog.AnyError(_logger, exception);
            throw;
        }
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dataSource
                .CountAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not ArgumentNullException
                                              and not OperationCanceledException)
        {
            QueryLog.CountError(_logger, exception);
            throw;
        }
    }

    public async Task<TResult> FirstAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dataSource
                .FirstAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not ArgumentNullException
                                              and not InvalidOperationException
                                              and not OperationCanceledException)
        {
            QueryLog.FirstError(_logger, exception);
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
        catch (Exception exception) when (exception is not ArgumentNullException
                                              and not OperationCanceledException)
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
        catch (Exception exception) when (exception is not ArgumentNullException
                                              and not OperationCanceledException)
        {
            QueryLog.ListError(_logger, exception);
            throw;
        }
    }

    public async Task<TResult> SingleAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dataSource
                .SingleAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not ArgumentNullException
                                              and not InvalidOperationException
                                              and not OperationCanceledException)
        {
            QueryLog.SingleError(_logger, exception);
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
        catch (Exception exception) when (exception is not ArgumentNullException
                                              and not InvalidOperationException
                                              and not OperationCanceledException)
        {
            QueryLog.SingleError(_logger, exception);
            throw;
        }
    }

    public async IAsyncEnumerable<TResult> ToAsyncEnumerable(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        IAsyncEnumerable<TResult> enumerable;
        try
        {
            enumerable = _dataSource.AsAsyncEnumerable();
        }
        catch (Exception exception) when (exception is not ArgumentNullException
                                              and not InvalidOperationException)
        {
            QueryLog.AsyncEnumerableError(_logger, exception);
            throw;
        }

        await foreach (TResult result in enumerable
                           .WithCancellation(cancellationToken)
                           .ConfigureAwait(false))
        {
            yield return result;
        }
    }
}
