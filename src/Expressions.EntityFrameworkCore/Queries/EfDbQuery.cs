using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Queries.Paging;

namespace Raiqub.Expressions.EntityFrameworkCore.Queries;

/// <summary>
/// Entity Framework-based implementation of a query that can be executed to retrieve instances of type <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TResult">The type of the result returned.</typeparam>
public class EfDbQuery<TResult> : IDbQuery<TResult>
{
    private readonly ILogger _logger;
    private readonly IQueryable<TResult> _dataSource;

    /// <summary>Initializes a new instance of the <see cref="EfDbQuery{TResult}"/> class.</summary>
    /// <param name="logger">The <see cref="ILogger"/> to log to.</param>
    /// <param name="dataSource">The data source to query from.</param>
    public EfDbQuery(
        ILogger logger,
        IQueryable<TResult> dataSource)
    {
        _logger = logger;
        _dataSource = dataSource;
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
    public async Task<long> LongCountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dataSource
                .LongCountAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not ArgumentNullException
                                              and not OperationCanceledException)
        {
            QueryLog.CountError(_logger, exception);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<TPage> ToPagedListAsync<TPage>(
        int pageNumber,
        int pageSize,
        PagedResultFactory<TResult, TPage> pagedResultFactory,
        CancellationToken cancellationToken = default)
    {
        var pagedQuery = _dataSource.PrepareQueryForPaging(pageNumber, pageSize);

        long totalCount;
        IReadOnlyList<TResult> items = Array.Empty<TResult>();
        try
        {
            totalCount = await _dataSource
                .LongCountAsync(cancellationToken)
                .ConfigureAwait(false);

            if (PageInfo.PageNumberExists(pageNumber, pageSize, totalCount))
            {
                items = await pagedQuery
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                totalCount = 0L;
            }
        }
        catch (Exception exception) when (exception is not ArgumentNullException
                                              and not OperationCanceledException)
        {
            QueryLog.PagedListError(_logger, exception);
            throw;
        }

        QueryLog.PagedListCountInfo(_logger, items.Count, totalCount);
        return pagedResultFactory(new PageInfo(pageNumber, pageSize, totalCount), items);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TResult>> ToListAsync(CancellationToken cancellationToken = default)
    {
        List<TResult> items;
        try
        {
            items = await _dataSource
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not ArgumentNullException
                                              and not OperationCanceledException)
        {
            QueryLog.ListError(_logger, exception);
            throw;
        }

        QueryLog.ListCountInfo(_logger, items.Count);
        return items;
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
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
