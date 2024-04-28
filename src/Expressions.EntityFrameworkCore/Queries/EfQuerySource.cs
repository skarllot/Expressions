using Microsoft.EntityFrameworkCore;
using Raiqub.Expressions.EntityFrameworkCore.Options;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Sessions;

#pragma warning disable CS0618 // Type or member is obsolete

namespace Raiqub.Expressions.EntityFrameworkCore.Queries;

/// <summary>Entity framework-based implementation of provider of data sources.</summary>
public class EfQuerySource : IQuerySource
{
    private readonly DbContext _dbContext;
    private readonly ISqlProviderSelector _sqlProviderSelector;
    private readonly EntityOptionsSelector _entityOptionsSelector;
    private readonly ChangeTracking _tracking;

    /// <summary>Initializes a new instance of the <see cref="EfQuerySource"/> class.</summary>
    /// <param name="dbContext">The <see cref="Microsoft.EntityFrameworkCore.DbContext"/> to read/write.</param>
    /// <param name="sqlProviderSelector">A selector to retrieve custom SQL for querying entities.</param>
    /// <param name="entityOptionsSelector">A selector to retrieve options for handling entities.</param>
    /// <param name="tracking">The change tracking mode of the session.</param>
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

    /// <inheritdoc />
    public IQueryable<TEntity> GetSet<TEntity>() where TEntity : class
    {
        EntityOptions? options = _entityOptionsSelector.GetOptions<TEntity>();

        return DataSourceFactory.GetDbSet<TEntity>(
            _dbContext,
            _sqlProviderSelector.GetQuerySql<TEntity>(),
            options?.ChangeTracking ?? _tracking,
            options?.UseSplitQuery);
    }

    /// <inheritdoc />
    public IQueryable<TEntity> GetSetFromSql<TEntity>(FormattableString sql) where TEntity : class
    {
        EntityOptions? options = _entityOptionsSelector.GetOptions<TEntity>();

        return DataSourceFactory.GetDbSet<TEntity>(
            _dbContext,
            SqlString.FromSqlInterpolated(sql),
            options?.ChangeTracking ?? _tracking,
            options?.UseSplitQuery);
    }

    /// <inheritdoc />
    public IQueryable<TEntity> GetSetFromRawSql<TEntity>(string sql, params object[] parameters) where TEntity : class
    {
        EntityOptions? options = _entityOptionsSelector.GetOptions<TEntity>();

        return DataSourceFactory.GetDbSet<TEntity>(
            _dbContext,
            SqlString.FromSqlRaw(sql, parameters),
            options?.ChangeTracking ?? _tracking,
            options?.UseSplitQuery);
    }

    /// <inheritdoc />
    public IQueryable<TResult> GetNonMappedFromSql<TResult>(FormattableString sql)
    {
#if NET8_0_OR_GREATER
        return _dbContext.Database.SqlQuery<TResult>(sql);
#else
        throw new NotSupportedException();
#endif
    }

    /// <inheritdoc />
    public IQueryable<TResult> GetNonMappedFromRawSql<TResult>(string sql, params object[] parameters)
    {
#if NET8_0_OR_GREATER
        return _dbContext.Database.SqlQueryRaw<TResult>(sql, parameters);
#else
        throw new NotSupportedException();
#endif
    }
}
