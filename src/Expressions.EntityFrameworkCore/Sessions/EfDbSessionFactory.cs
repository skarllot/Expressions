using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Sessions;
using Raiqub.Expressions.Sessions.BoundedContext;

namespace Raiqub.Expressions.EntityFrameworkCore.Sessions;

public class EfDbSessionFactory<TContext>
    : IDbSessionFactory<TContext>, IDbQuerySessionFactory<TContext>, IDbSessionFactory, IDbQuerySessionFactory
    where TContext : DbContext
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly IDbContextFactory<TContext> _contextFactory;

    public EfDbSessionFactory(ILoggerFactory loggerFactory, IDbContextFactory<TContext> contextFactory)
    {
        _loggerFactory = loggerFactory;
        _contextFactory = contextFactory;
    }

    public EfDbSession<TContext> Create(ChangeTracking? tracking = null) => new(
        _loggerFactory.CreateLogger<EfDbSession<TContext>>(),
        _contextFactory.CreateDbContext(),
        tracking ?? ChangeTracking.Default);

    public EfDbSession<TContext> CreateForQuery() => Create(ChangeTracking.Disable);

    IDbSession IDbSessionFactory.Create(ChangeTracking? tracking) => Create(tracking);

    IDbSession<TContext> IDbSessionFactory<TContext>.Create(ChangeTracking? tracking) => Create(tracking);

    IDbQuerySession IDbQuerySessionFactory.Create() => CreateForQuery();

    IDbQuerySession<TContext> IDbQuerySessionFactory<TContext>.Create() => CreateForQuery();
}
