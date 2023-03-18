using Marten;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Sessions;
using Raiqub.Expressions.Sessions.BoundedContext;

namespace Raiqub.Expressions.Marten.Sessions.BoundedContext;

public class MartenSessionFactory<TContext> : ISessionFactory<TContext>, IQuerySessionFactory<TContext>
    where TContext : class, IDocumentStore
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly TContext _context;

    public MartenSessionFactory(ILoggerFactory loggerFactory, TContext context)
    {
        _loggerFactory = loggerFactory;
        _context = context;
    }

    public ISession<TContext> Create(ChangeTracking? tracking = null) => new MartenSession<TContext>(
        _loggerFactory.CreateLogger<MartenSession<TContext>>(),
        MartenSessionFactory.CreateSession(_context, tracking ?? ChangeTracking.Default),
        tracking ?? ChangeTracking.Default,
        _context);

    public IQuerySession<TContext> CreateForQuery() => new MartenQuerySession<TContext>(
        _loggerFactory.CreateLogger<MartenQuerySession<TContext>>(),
        _context.QuerySession(),
        _context);

    IQuerySession<TContext> IQuerySessionFactory<TContext>.Create() => CreateForQuery();
}
