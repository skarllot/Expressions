using Marten;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Sessions;
using Raiqub.Expressions.Sessions.BoundedContext;

namespace Raiqub.Expressions.Marten.Sessions.BoundedContext;

public class MartenSession<TContext> : MartenSession, ISession<TContext>
    where TContext : IDocumentStore
{
    public MartenSession(
        ILogger<MartenSession<TContext>> logger,
        IDocumentSession session,
        ChangeTracking tracking,
        TContext context)
        : base(logger, session, tracking)
    {
        Context = context;
    }

    public TContext Context { get; }
}
