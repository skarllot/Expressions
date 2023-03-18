using Marten;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.Marten.Queries;

public class MartenQuery<TSource, TResult> : IQuery<TResult>
{
    private readonly ILogger _logger;
    private readonly IQuerySession _session;
    private readonly IQueryModel<TSource, TResult> _queryModel;

    public MartenQuery(
        ILogger logger,
        IQuerySession session,
        IQueryModel<TSource, TResult> queryModel)
    {
        _logger = logger;
        _session = session;
        _queryModel = queryModel;
    }

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _session
                .Query<TSource>()
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
            return await _session
                .Query<TSource>()
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
            return await _session
                .Query<TSource>()
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
            return await _session
                .Query<TSource>()
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
            return await _session
                .Query<TSource>()
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
}
