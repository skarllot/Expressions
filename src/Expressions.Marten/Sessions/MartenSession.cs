using Marten;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Expressions.Marten.Sessions;

public class MartenSession : MartenQuerySession, ISession
{
    private readonly IDocumentSession _session;

    public MartenSession(ILogger<MartenSession> logger, IDocumentSession session, ChangeTracking tracking)
        : base(logger, session)
    {
        _session = session;
        Tracking = tracking;
    }

    public override ChangeTracking Tracking { get; }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _session.SaveChangesAsync(cancellationToken);
}
