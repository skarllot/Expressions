using Microsoft.EntityFrameworkCore;
using Raiqub.Expressions.Repositories;

namespace Raiqub.Expressions.EntityFrameworkCore.Repositories;

public class DbSession<TDbContext> : ISession
    where TDbContext : DbContext
{
    public DbSession(TDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public DbContext DbContext { get; }
    public ChangeTracking? Tracking { get; private set; }

    public void Open(ChangeTracking? tracking = null)
    {
        Tracking = tracking;
    }

    public void OpenForQuery()
    {
        Tracking = ChangeTracking.Disable;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return DbContext.SaveChangesAsync(true, cancellationToken);
    }
}
