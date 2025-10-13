using System.Runtime.CompilerServices;
using JasperFx.Core.Reflection;
using Marten;
using Marten.Linq;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Queries.Paging;

namespace Raiqub.Expressions.Marten.Queries;

/// <summary>
/// Provides a base implementation for Marten database queries that can be executed to retrieve instances of type <typeparamref name="TResult"/>.
/// </summary>
/// <typeparam name="TResult">The type of the result returned by the query.</typeparam>
public abstract class MartenDbQueryBase<TResult> : IDbQueryBase<TResult>
    where TResult : notnull
{
    private readonly ILogger _logger;
    private readonly IQueryable<TResult> _dataSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="MartenDbQueryBase{TResult}"/> class.
    /// </summary>
    /// <param name="logger">The logger instance to use for logging operations.</param>
    /// <param name="dataSource">The queryable data source to execute queries against.</param>
    protected MartenDbQueryBase(ILogger logger, IQueryable<TResult> dataSource)
    {
        _logger = logger;
        _dataSource = dataSource;
    }

    /// <summary>
    /// Gets the logger instance used for logging operations.
    /// </summary>
    protected ILogger Logger
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _logger;
    }

    /// <summary>
    /// Gets the queryable data source used for executing queries.
    /// </summary>
    protected IQueryable<TResult> DataSource
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _dataSource;
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
    public Task<PagedResult<TResult>> ToPagedListAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return ToPagedListAsync(pageNumber, pageSize, DefaultPagedResultFactory<TResult>.Create, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TPage> ToPagedListAsync<TPage>(
        int pageNumber,
        int pageSize,
        PagedResultFactory<TResult, TPage> pagedResultFactory,
        CancellationToken cancellationToken = default)
    {
        var queryable = _dataSource.As<IMartenQueryable<TResult>>()
            .Stats(out QueryStatistics stats)
            .PrepareQueryForPaging(pageNumber, pageSize);

        IReadOnlyList<TResult> items;
        try
        {
            items = await queryable
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not ArgumentNullException
                                              and not OperationCanceledException)
        {
            QueryLog.PagedListError(_logger, exception);
            throw;
        }

        QueryLog.PagedListCountInfo(_logger, items.Count, stats.TotalResults);
        return pagedResultFactory(new PageInfo(pageNumber, pageSize, stats.TotalResults), items);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TResult>> ToListAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<TResult> items;
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
    public IAsyncEnumerable<TResult> ToAsyncEnumerable(CancellationToken cancellationToken = default)
    {
        return _dataSource.ToAsyncEnumerable(cancellationToken);
    }
}
