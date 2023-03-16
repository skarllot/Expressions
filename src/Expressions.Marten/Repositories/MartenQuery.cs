using Marten;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Repositories;

namespace Raiqub.Expressions.Marten.Repositories;

public class MartenQuery<TSource, TResult> : IQuery<TResult>
{
    private readonly ILogger _logger;
    private readonly IDocumentStore _documentStore;
    private readonly IQuerySession? _session;
    private readonly QueryModel<TSource, TResult> _queryModel;

    public MartenQuery(
        ILogger logger,
        IDocumentStore documentStore,
        IQuerySession? session,
        QueryModel<TSource, TResult> queryModel)
    {
        _logger = logger;
        _documentStore = documentStore;
        _session = session;
        _queryModel = queryModel;
    }

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        var disposable = GetOrOpenForQuery(out var session);

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
        var disposable = GetOrOpenForQuery(out var session);

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
        var disposable = GetOrOpenForQuery(out var session);

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
        var disposable = GetOrOpenForQuery(out var session);

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
            QueryLog.ListError(_logger, exception);
            throw;
        }
        finally
        {
            if (disposable is not null)
                await disposable.DisposeAsync().ConfigureAwait(false);
        }
    }

    public async Task<TResult?> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        var disposable = GetOrOpenForQuery(out var session);

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
            QueryLog.SingleError(_logger, exception);
            throw;
        }
        finally
        {
            if (disposable is not null)
                await disposable.DisposeAsync().ConfigureAwait(false);
        }
    }

    private IAsyncDisposable? GetOrOpenForQuery(out IQuerySession session)
    {
        if (_session is not null)
        {
            session = _session;
            return null;
        }

        session = _documentStore.QuerySession();
        return session;
    }
}
