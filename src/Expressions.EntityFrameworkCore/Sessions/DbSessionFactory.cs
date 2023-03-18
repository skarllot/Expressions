using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Sessions;
using Raiqub.Expressions.Sessions.BoundedContext;

namespace Raiqub.Expressions.EntityFrameworkCore.Sessions;

public class DbSessionFactory<TContext>
    : ISessionFactory<TContext>, IQuerySessionFactory<TContext>, ISessionFactory, IQuerySessionFactory
    where TContext : DbContext
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly TContext _context;

    public DbSessionFactory(ILoggerFactory loggerFactory, TContext context)
    {
        _loggerFactory = loggerFactory;
        _context = context;
    }

    public DbSession<TContext> Create(ChangeTracking? tracking = null) => new(
        _loggerFactory.CreateLogger<DbSession<TContext>>(),
        _context,
        tracking ?? ChangeTracking.Default);

    public DbSession<TContext> CreateForQuery() => Create(ChangeTracking.Disable);

    ISession ISessionFactory.Create(ChangeTracking? tracking) => Create(tracking);

    ISession<TContext> ISessionFactory<TContext>.Create(ChangeTracking? tracking) => Create(tracking);

    IQuerySession IQuerySessionFactory.Create() => CreateForQuery();

    IQuerySession<TContext> IQuerySessionFactory<TContext>.Create() => CreateForQuery();
}
