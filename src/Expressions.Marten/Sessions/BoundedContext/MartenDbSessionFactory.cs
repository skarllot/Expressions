using Marten;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Sessions;
using Raiqub.Expressions.Sessions.BoundedContext;

namespace Raiqub.Expressions.Marten.Sessions.BoundedContext;

/// <summary>Marten-based implementation of a factory for creating database sessions for data access.</summary>
/// <typeparam name="TContext">The type of the bounded context.</typeparam>
public class MartenDbSessionFactory<TContext> : IDbSessionFactory<TContext>, IDbQuerySessionFactory<TContext>
    where TContext : class, IDocumentStore
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly TContext _context;

    /// <summary>Initializes a new instance of the <see cref="MartenDbSessionFactory{TContext}"/> class.</summary>
    /// <param name="loggerFactory">The factory used to create loggers.</param>
    /// <param name="context">The <see cref="IDocumentSession"/> to read/write.</param>
    public MartenDbSessionFactory(ILoggerFactory loggerFactory, TContext context)
    {
        _loggerFactory = loggerFactory;
        _context = context;
    }

    /// <summary>Creates a new database session.</summary>
    /// <param name="tracking">The change tracking mode of the new session.</param>
    /// <returns>A new database session.</returns>
    public IDbSession<TContext> Create(ChangeTracking? tracking = null) => new MartenDbSession<TContext>(
        _loggerFactory.CreateLogger<MartenDbSession<TContext>>(),
        MartenDbSessionFactory.CreateSession(_context, tracking ?? ChangeTracking.Default),
        tracking ?? ChangeTracking.Default,
        _context);

    /// <summary>Creates a new database query session.</summary>
    /// <returns>A new database query session.</returns>
    public IDbQuerySession<TContext> CreateForQuery() => new MartenDbQuerySession<TContext>(
        _loggerFactory.CreateLogger<MartenDbQuerySession<TContext>>(),
        _context.QuerySession(),
        _context);

    IDbQuerySession<TContext> IDbQuerySessionFactory<TContext>.Create() => CreateForQuery();
}
