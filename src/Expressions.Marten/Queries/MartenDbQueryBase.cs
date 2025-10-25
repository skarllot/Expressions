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
    private readonly DbQueryScope _dbQueryScope;
    private readonly IQueryable<TResult> _dataSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="MartenDbQueryBase{TResult}"/> class.
    /// </summary>
    /// <param name="logger">The logger instance to use for logging operations.</param>
    /// <param name="dbQueryScope">The query scope information for logging context.</param>
    /// <param name="dataSource">The queryable data source to execute queries against.</param>
    protected MartenDbQueryBase(ILogger logger, DbQueryScope dbQueryScope, IQueryable<TResult> dataSource)
    {
        _logger = logger;
        _dbQueryScope = dbQueryScope;
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
        using (BeginLogScope())
        {
            return await _dataSource
                .AnyAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        using (BeginLogScope())
        {
            return await _dataSource
                .CountAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async Task<TResult> FirstAsync(CancellationToken cancellationToken = default)
    {
        using (BeginLogScope())
        {
            return await _dataSource
                .FirstAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async Task<long> LongCountAsync(CancellationToken cancellationToken = default)
    {
        using (BeginLogScope())
        {
            return await _dataSource
                .LongCountAsync(cancellationToken)
                .ConfigureAwait(false);
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
        using (BeginLogScope())
        {
            var queryable = _dataSource.As<IMartenQueryable<TResult>>()
                .Stats(out QueryStatistics stats)
                .PrepareQueryForPaging(pageNumber, pageSize);

            var items = await queryable
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            QueryLog.PagedListCountInfo(_logger, items.Count, stats.TotalResults);
            return pagedResultFactory(new PageInfo(pageNumber, pageSize, stats.TotalResults), items);
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TResult>> ToListAsync(CancellationToken cancellationToken = default)
    {
        using (BeginLogScope())
        {
            var items = await _dataSource
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            QueryLog.ListCountInfo(_logger, items.Count);
            return items;
        }
    }

    /// <inheritdoc />
    public async Task<TResult> SingleAsync(CancellationToken cancellationToken = default)
    {
        using (BeginLogScope())
        {
            return await _dataSource
                .SingleAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<TResult> ToAsyncEnumerable(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using (BeginLogScope())
        {
            await foreach (TResult result in _dataSource
                               .ToAsyncEnumerable(cancellationToken)
                               .ConfigureAwait(false))
            {
                yield return result;
            }
        }
    }

    /// <summary>Begins a logical operation scope with query scope information for logging purposes.</summary>
    /// <returns>A disposable object that ends the logical operation scope on dispose, or <c>null</c> if the logger does not support scopes.</returns>
    protected IDisposable? BeginLogScope() => _logger.BeginScope(_dbQueryScope);
}
