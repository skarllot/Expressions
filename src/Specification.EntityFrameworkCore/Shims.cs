namespace Raiqub.Specification.EntityFrameworkCore;

internal static class Shims
{
#if !CHANGE_TRACKER_IDENTITY_RESOLUTION
    public static IQueryable<TEntity> AsNoTrackingWithIdentityResolution<TEntity>(this IQueryable<TEntity> source)
    {
        throw new InvalidOperationException(
            "The current version of Entity Framework Core does not support no tracking with identity resolution");
    }
#endif
}
