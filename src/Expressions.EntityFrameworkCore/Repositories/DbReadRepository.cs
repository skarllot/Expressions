using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Repositories;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Expressions.EntityFrameworkCore.Repositories;

public class DbReadRepository<TContext, TEntity> : IReadRepository<TContext, TEntity>
    where TContext : DbContext
    where TEntity : class
{
    private readonly ILogger<DbReadRepository<TContext, TEntity>> _logger;
    private readonly TContext _context;

    public DbReadRepository(ILogger<DbReadRepository<TContext, TEntity>> logger, TContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IQuery<TResult> Query<TResult>(
        QueryModel<TEntity, TResult> queryModel,
        IReadSession<TContext>? session = null)
    {
        return new DbQuery<TEntity, TResult>(
            _logger,
            session?.Context ?? _context,
            queryModel,
            session?.Tracking ?? ChangeTracking.Default);
    }
}
