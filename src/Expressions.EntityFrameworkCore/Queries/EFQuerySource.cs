using Microsoft.EntityFrameworkCore;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Expressions.EntityFrameworkCore.Queries;

public class EFQuerySource : IQuerySource
{
    private readonly DbContext _dbContext;
    private readonly ChangeTracking _tracking;

    public EFQuerySource(DbContext dbContext, ChangeTracking tracking)
    {
        _dbContext = dbContext;
        _tracking = tracking;
    }

    public IQueryable<TEntity> GetSet<TEntity>() where TEntity : class
    {
        return DataSourceFactory.GetDbSet<TEntity>(_dbContext, _tracking);
    }
}
