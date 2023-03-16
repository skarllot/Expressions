using Marten;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Expressions.Marten.Sessions;

public class MartenSession<TContext> : MartenReadSession<TContext>, ISession<TContext>
    where TContext : IDocumentStore
{
    public MartenSession(TContext context, ChangeTracking? tracking)
        : base(context, CreateSession(context, tracking ?? ChangeTracking.Default))
    {
        Tracking = tracking;
    }

    public override ChangeTracking? Tracking { get; }

#if NET6_0_OR_GREATER
    public override IDocumentSession Session => (IDocumentSession)base.Session;
#else
    public new IDocumentSession Session => (IDocumentSession)base.Session;
#endif

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return Session.SaveChangesAsync(cancellationToken);
    }

    private static IDocumentSession CreateSession(TContext context, ChangeTracking tracking)
    {
        return tracking switch
        {
            ChangeTracking.Default => context.OpenSession(),
            ChangeTracking.Enable => context.OpenSession(DocumentTracking.DirtyTracking),
            ChangeTracking.IdentityResolution => context.OpenSession(),
            ChangeTracking.Disable => context.OpenSession(DocumentTracking.None),
            _ => throw new ArgumentException(
                $"The specified change tracking mode is not supported: {tracking}",
                nameof(tracking))
        };
    }
}
