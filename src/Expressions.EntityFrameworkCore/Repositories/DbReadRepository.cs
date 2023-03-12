using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Repositories;

namespace Raiqub.Expressions.EntityFrameworkCore.Repositories;

public class DbReadRepository<TDbContext, TEntity> : IReadRepository<TEntity>
    where TDbContext : DbContext
    where TEntity : class
{
    private readonly ILogger<DbReadRepository<TDbContext, TEntity>> _logger;
    private readonly DbSession<TDbContext> _session;

    public DbReadRepository(ILogger<DbReadRepository<TDbContext, TEntity>> logger, DbSession<TDbContext> session)
    {
        _logger = logger;
        _session = session;
    }

    public ISession Session => _session;

    public IQuery<TResult> Query<TResult>(QueryModel<TEntity, TResult> queryModel, ChangeTracking? tracking = null)
    {
        return new DbQuery<TEntity, TResult>(
            _logger,
            _session.DbContext,
            queryModel,
            tracking ?? _session.Tracking ?? ChangeTracking.Default);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual ValueTask DisposeAsyncCore()
    {
        // Nothing to clean
        return new ValueTask();
    }

    protected virtual void Dispose(bool disposing)
    {
        // Nothing to clean
    }
}
