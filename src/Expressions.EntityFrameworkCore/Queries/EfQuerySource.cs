using Microsoft.EntityFrameworkCore;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Expressions.EntityFrameworkCore.Queries;

public class EfQuerySource : IQuerySource
{
    private readonly DbContext _dbContext;
    private readonly ChangeTracking _tracking;

    public EfQuerySource(DbContext dbContext, ChangeTracking tracking)
    {
        _dbContext = dbContext;
        _tracking = tracking;
    }

    public IQueryable<TEntity> GetSet<TEntity>() where TEntity : class
    {
        return DataSourceFactory.GetDbSet<TEntity>(_dbContext, _tracking);
    }
}
