using Marten;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Repositories;

namespace Raiqub.Expressions.Marten.Repositories;

public class MartenQuery<TSource, TResult> : IQuery<TResult>
{
    private readonly ILogger _logger;
    private readonly MartenSession _session;
    private readonly QueryModel<TSource, TResult> _queryModel;
    private readonly ChangeTracking _tracking;

    public MartenQuery(
        ILogger logger,
        MartenSession session,
        QueryModel<TSource, TResult> queryModel,
        ChangeTracking tracking)
    {
        _logger = logger;
        _session = session;
        _queryModel = queryModel;
        _tracking = tracking;
    }

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        var disposable = _session.GetOrOpenForQuery(out var session);

        try
        {
            return await session
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
        finally
        {
            if (disposable is not null)
                await disposable.DisposeAsync().ConfigureAwait(false);
        }
    }

    public async Task<long> CountAsync(CancellationToken cancellationToken = default)
    {
        var disposable = _session.GetOrOpenForQuery(out var session);

        try
        {
            return await session
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
        finally
        {
            if (disposable is not null)
                await disposable.DisposeAsync().ConfigureAwait(false);
        }
    }

    public async Task<TResult?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        var disposable = _session.GetOrOpenForQuery(out var session);

        try
        {
            return await session
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
        finally
        {
            if (disposable is not null)
                await disposable.DisposeAsync().ConfigureAwait(false);
        }
    }

    public async Task<IReadOnlyList<TResult>> ToListAsync(CancellationToken cancellationToken = default)
    {
        var session = CreateSession(_tracking);
        await using (session.ConfigureAwait(false))
        {
            try
            {
                return await session
                    .Query<TSource>()
                    .Apply(_queryModel)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                _logger.LogError(exception, "Error trying to list found elements");
                throw;
            }
        }
    }

    public async Task<TResult?> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        var session = CreateSession(_tracking);
        await using (session.ConfigureAwait(false))
        {
            try
            {
                return await session
                    .Query<TSource>()
                    .Apply(_queryModel)
                    .SingleOrDefaultAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                _logger.LogError(exception, "Error trying to query the single element");
                throw;
            }
        }
    }

    protected IQuerySession CreateSession(ChangeTracking tracking = ChangeTracking.Default)
    {
        return tracking switch
        {
            ChangeTracking.Default => _documentStore.OpenSession(),
            ChangeTracking.Enable => _documentStore.OpenSession(DocumentTracking.DirtyTracking),
            ChangeTracking.IdentityResolution => _documentStore.OpenSession(),
            ChangeTracking.Disable => _documentStore.OpenSession(DocumentTracking.None),
            _ => throw new ArgumentException(
                $"The specified change tracking mode is not supported: {tracking}",
                nameof(tracking))
        };
    }
}
