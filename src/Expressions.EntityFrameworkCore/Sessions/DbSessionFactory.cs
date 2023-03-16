using Microsoft.EntityFrameworkCore;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Expressions.EntityFrameworkCore.Sessions;

public class DbSessionFactory<TContext> : ISessionFactory<TContext>
    where TContext : DbContext
{
    private readonly TContext _context;

    public DbSessionFactory(TContext context) => _context = context;

    public ISession<TContext> Create(ChangeTracking? tracking = null) => new DbSession<TContext>(_context, tracking);

    public IReadSession<TContext> CreateForQuery() => new DbSession<TContext>(_context, ChangeTracking.Disable);
}
