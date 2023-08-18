using Marten;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Sessions;
using Raiqub.Expressions.Sessions.BoundedContext;

namespace Raiqub.Expressions.Marten.Sessions.BoundedContext;

public class MartenDbSession<TContext> : MartenDbSession, IDbSession<TContext>
    where TContext : IDocumentStore
{
    public MartenDbSession(
        ILogger<MartenDbSession<TContext>> logger,
        IDocumentSession session,
        ChangeTracking tracking,
        TContext context)
        : base(logger, session, tracking)
    {
        Context = context;
    }

    public TContext Context { get; }
}
