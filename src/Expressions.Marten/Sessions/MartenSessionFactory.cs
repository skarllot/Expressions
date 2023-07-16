using Marten;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Sessions;
using IQuerySession = Raiqub.Expressions.Sessions.IQuerySession;
using ISessionFactory = Raiqub.Expressions.Sessions.ISessionFactory;

namespace Raiqub.Expressions.Marten.Sessions;

public class MartenSessionFactory : ISessionFactory, IQuerySessionFactory
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly IDocumentStore _documentStore;

    public MartenSessionFactory(ILoggerFactory loggerFactory, IDocumentStore documentStore)
    {
        _loggerFactory = loggerFactory;
        _documentStore = documentStore;
    }

    public ISession Create(ChangeTracking? tracking = null) => new MartenSession(
        _loggerFactory.CreateLogger<MartenSession>(),
        CreateSession(_documentStore, tracking ?? ChangeTracking.Default),
        tracking ?? ChangeTracking.Default);

    public IQuerySession CreateForQuery() => new MartenQuerySession(
        _loggerFactory.CreateLogger<MartenSession>(),
        _documentStore.QuerySession());

    IQuerySession IQuerySessionFactory.Create() => CreateForQuery();

    internal static IDocumentSession CreateSession(IDocumentStore documentStore, ChangeTracking tracking)
    {
        return tracking switch
        {
            ChangeTracking.Default => documentStore.LightweightSession(),
            ChangeTracking.Enable => documentStore.DirtyTrackedSession(),
            ChangeTracking.IdentityResolution => documentStore.IdentitySession(),
            ChangeTracking.Disable => documentStore.LightweightSession(),
            _ => throw new ArgumentException(
                $"The specified change tracking mode is not supported: {tracking}",
                nameof(tracking))
        };
    }
}
