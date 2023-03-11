using Microsoft.EntityFrameworkCore;
using Raiqub.Expressions.Repositories;

namespace Raiqub.Expressions.EntityFrameworkCore.Repositories;

public class DbSessionFactory<TDbContext> : ISessionFactory
    where TDbContext : DbContext
{
    private readonly DbContext _dbContext;

    public DbSessionFactory(TDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public ISession Create(ChangeTracking? tracking = null) => throw new NotImplementedException();
    public IQuerySession CreateForQuery() => throw new NotImplementedException();
}
