using Microsoft.EntityFrameworkCore;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Expressions.EntityFrameworkCore.Queries;

internal static class DataSourceFactory
{
    public static IQueryable<TEntity> GetDbSet<TEntity>(
        DbContext dbContext,
        SqlString? querySql,
        ChangeTracking tracking,
        bool? useSplitQuery)
        where TEntity : class
    {
        DbSet<TEntity> dbSet = dbContext.Set<TEntity>();

        IQueryable<TEntity> queryable;
        if (querySql is not null)
        {
            queryable = querySql.Value.IsRaw
                ? dbSet.FromSqlRaw(querySql.Value.Sql.Format, querySql.Value.Sql.GetArguments()!)
                : dbSet.FromSqlInterpolated(querySql.Value.Sql);
        }
        else
        {
            queryable = dbSet;
        }

        queryable = tracking switch
        {
            ChangeTracking.Default => queryable,
            ChangeTracking.Enable => queryable.AsTracking(),
            ChangeTracking.Disable => queryable.AsNoTracking(),
            ChangeTracking.IdentityResolution => queryable.AsNoTrackingWithIdentityResolution(),
            _ => throw new ArgumentException(
                $"The specified change tracking mode is not supported: {tracking}",
                nameof(tracking))
        };

        if (useSplitQuery == true)
        {
            queryable = queryable.AsSplitQuery();
        }

        return queryable;
    }
}
