using Marten;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Sessions.BoundedContext;

namespace Raiqub.Expressions.Marten.Sessions.BoundedContext;

public class MartenDbQuerySession<TContext> : MartenDbQuerySession, IDbQuerySession<TContext>
    where TContext : IDocumentStore
{
    public MartenDbQuerySession(ILogger<MartenDbQuerySession<TContext>> logger, IQuerySession session, TContext context)
        : base(logger, session)
    {
        Context = context;
    }

    public TContext Context { get; }
}
