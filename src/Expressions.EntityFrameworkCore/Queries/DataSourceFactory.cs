using Microsoft.EntityFrameworkCore;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Expressions.EntityFrameworkCore.Queries;

internal static class DataSourceFactory
{
    public static IQueryable<TSource> GetDbSet<TSource>(
        DbContext dbContext,
        ChangeTracking tracking = ChangeTracking.Default)
        where TSource : class
    {
        IQueryable<TSource> queryable = dbContext.Set<TSource>();

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

        return queryable;
    }
}
