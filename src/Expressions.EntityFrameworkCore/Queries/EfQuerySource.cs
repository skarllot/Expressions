using Microsoft.EntityFrameworkCore;
using Raiqub.Expressions.EntityFrameworkCore.Options;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Expressions.EntityFrameworkCore.Queries;

public class EfQuerySource : IQuerySource
{
    private readonly DbContext _dbContext;
    private readonly ISqlProviderSelector _sqlProviderSelector;
    private readonly EntityOptionsSelector _entityOptionsSelector;
    private readonly ChangeTracking _tracking;

    public EfQuerySource(
        DbContext dbContext,
        ISqlProviderSelector sqlProviderSelector,
        EntityOptionsSelector entityOptionsSelector,
        ChangeTracking tracking)
    {
        _dbContext = dbContext;
        _sqlProviderSelector = sqlProviderSelector;
        _entityOptionsSelector = entityOptionsSelector;
        _tracking = tracking;
    }

    public IQueryable<TEntity> GetSet<TEntity>() where TEntity : class
    {
        EntityOptions? options = _entityOptionsSelector.GetOptions<TEntity>();
        if (options is null)
        {
            return DataSourceFactory.GetDbSet<TEntity>(
                _dbContext,
                _sqlProviderSelector.GetQuerySql<TEntity>(),
                _tracking);
        }

        var dbSet = DataSourceFactory.GetDbSet<TEntity>(
            _dbContext,
            _sqlProviderSelector.GetQuerySql<TEntity>(),
            options.ChangeTracking ?? _tracking);

        return options.UseSplitQuery ? dbSet.AsSplitQuery() : dbSet;
    }
}
