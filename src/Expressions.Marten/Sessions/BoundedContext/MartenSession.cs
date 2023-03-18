using Marten;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Sessions;
using Raiqub.Expressions.Sessions.BoundedContext;

namespace Raiqub.Expressions.Marten.Sessions.BoundedContext;

public class MartenSession<TContext> : MartenQuerySession<TContext>, ISession<TContext>
    where TContext : IDocumentStore
{
    private readonly IDocumentSession _session;

    public MartenSession(
        ILogger<MartenSession<TContext>> logger,
        IDocumentSession session,
        ChangeTracking tracking,
        TContext context)
        : base(logger, session, context)
    {
        _session = session;
        Tracking = tracking;
    }

    public override ChangeTracking Tracking { get; }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _session.SaveChangesAsync(cancellationToken);
    }
}
