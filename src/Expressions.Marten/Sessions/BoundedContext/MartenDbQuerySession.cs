using Marten;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Sessions.BoundedContext;

namespace Raiqub.Expressions.Marten.Sessions.BoundedContext;

/// <summary>Marten-based implementation of a database session for querying instances.</summary>
/// <typeparam name="TContext">The type of the bounded context.</typeparam>
public class MartenDbQuerySession<TContext> : MartenDbQuerySession, IDbQuerySession<TContext>
    where TContext : IDocumentStore
{
    /// <summary>Initializes a new instance of the <see cref="MartenDbQuerySession{TContext}"/> class.</summary>
    /// <param name="logger">The <see cref="ILogger"/> to log to.</param>
    /// <param name="session">The Marten session to query from.</param>
    /// <param name="context">The bounded context associated with this query session.</param>
    public MartenDbQuerySession(ILogger<MartenDbQuerySession<TContext>> logger, IQuerySession session, TContext context)
        : base(logger, session)
    {
        Context = context;
    }

    /// <inheritdoc />
    public TContext Context { get; }
}
