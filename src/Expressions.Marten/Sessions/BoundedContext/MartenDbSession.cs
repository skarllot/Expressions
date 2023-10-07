using Marten;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Sessions;
using Raiqub.Expressions.Sessions.BoundedContext;

namespace Raiqub.Expressions.Marten.Sessions.BoundedContext;

/// <summary>Marten-based implementation of a database session for querying and saving instances.</summary>
/// <typeparam name="TContext">The type of the bounded context.</typeparam>
public class MartenDbSession<TContext> : MartenDbSession, IDbSession<TContext>
    where TContext : IDocumentStore
{
    /// <summary>Initializes a new instance of the <see cref="MartenDbSession{TContext}"/> class.</summary>
    /// <param name="logger">The <see cref="ILogger"/> to log to.</param>
    /// <param name="session">The <see cref="IDocumentSession"/> to read/write.</param>
    /// <param name="tracking">The change tracking mode of the session.</param>
    /// <param name="context">The bounded context associated with this query session.</param>
    public MartenDbSession(
        ILogger<MartenDbSession<TContext>> logger,
        IDocumentSession session,
        ChangeTracking tracking,
        TContext context)
        : base(logger, session, tracking)
    {
        Context = context;
    }

    /// <inheritdoc />
    public TContext Context { get; }
}
