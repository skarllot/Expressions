using Marten;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Sessions.BoundedContext;
using IQuerySession = Marten.IQuerySession;

namespace Raiqub.Expressions.Marten.Sessions.BoundedContext;

public class MartenQuerySession<TContext> : MartenQuerySession, IQuerySession<TContext>
    where TContext : IDocumentStore
{
    public MartenQuerySession(ILogger<MartenQuerySession<TContext>> logger, IQuerySession session, TContext context)
        : base(logger, session)
    {
        Context = context;
    }

    public TContext Context { get; }
}
