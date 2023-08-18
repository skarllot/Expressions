using Marten;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Sessions;
using Raiqub.Expressions.Sessions.BoundedContext;

namespace Raiqub.Expressions.Marten.Sessions.BoundedContext;

public class MartenDbSessionFactory<TContext> : IDbSessionFactory<TContext>, IDbQuerySessionFactory<TContext>
    where TContext : class, IDocumentStore
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly TContext _context;

    public MartenDbSessionFactory(ILoggerFactory loggerFactory, TContext context)
    {
        _loggerFactory = loggerFactory;
        _context = context;
    }

    public IDbSession<TContext> Create(ChangeTracking? tracking = null) => new MartenDbSession<TContext>(
        _loggerFactory.CreateLogger<MartenDbSession<TContext>>(),
        MartenDbSessionFactory.CreateSession(_context, tracking ?? ChangeTracking.Default),
        tracking ?? ChangeTracking.Default,
        _context);

    public IDbQuerySession<TContext> CreateForQuery() => new MartenDbQuerySession<TContext>(
        _loggerFactory.CreateLogger<MartenDbQuerySession<TContext>>(),
        _context.QuerySession(),
        _context);

    IDbQuerySession<TContext> IDbQuerySessionFactory<TContext>.Create() => CreateForQuery();
}
