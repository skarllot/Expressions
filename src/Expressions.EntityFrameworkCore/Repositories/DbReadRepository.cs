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
    private readonly DbContext _dbContext;

    public DbReadRepository(ILogger<DbReadRepository<TDbContext, TEntity>> logger, TDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public IQuery<TResult> Using<TResult>(QueryModel<TEntity, TResult> queryModel, ChangeTracking? tracking = null)
    {
        return new DbQuery<TEntity, TResult>(
            _logger,
            _dbContext,
            queryModel,
            tracking ?? queryModel.DefaultChangeTracking ?? ChangeTracking.Default);
    }
}
