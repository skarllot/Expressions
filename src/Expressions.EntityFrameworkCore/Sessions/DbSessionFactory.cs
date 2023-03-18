using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Sessions;
using Raiqub.Expressions.Sessions.BoundedContext;

namespace Raiqub.Expressions.EntityFrameworkCore.Sessions;

public class DbSessionFactory<TContext> : ISessionFactory<TContext>, IQuerySessionFactory<TContext>
    where TContext : DbContext
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly TContext _context;

    public DbSessionFactory(ILoggerFactory loggerFactory, TContext context)
    {
        _loggerFactory = loggerFactory;
        _context = context;
    }

    public ISession<TContext> Create(ChangeTracking? tracking = null) => new DbSession<TContext>(
        _loggerFactory.CreateLogger<DbSession<TContext>>(),
        _context,
        tracking ?? ChangeTracking.Default);

    public IQuerySession<TContext> CreateForQuery() => new DbSession<TContext>(
        _loggerFactory.CreateLogger<DbSession<TContext>>(),
        _context,
        ChangeTracking.Disable);

    IQuerySession<TContext> IQuerySessionFactory<TContext>.Create() => CreateForQuery();
}
