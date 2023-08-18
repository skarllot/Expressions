using Marten;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Expressions.Marten.Sessions;

public class MartenDbSessionFactory : IDbSessionFactory, IDbQuerySessionFactory
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly IDocumentStore _documentStore;

    public MartenDbSessionFactory(ILoggerFactory loggerFactory, IDocumentStore documentStore)
    {
        _loggerFactory = loggerFactory;
        _documentStore = documentStore;
    }

    public IDbSession Create(ChangeTracking? tracking = null) => new MartenDbSession(
        _loggerFactory.CreateLogger<MartenDbSession>(),
        CreateSession(_documentStore, tracking ?? ChangeTracking.Default),
        tracking ?? ChangeTracking.Default);

    public IDbQuerySession CreateForQuery() => new MartenDbQuerySession(
        _loggerFactory.CreateLogger<MartenDbSession>(),
        _documentStore.QuerySession());

    IDbQuerySession IDbQuerySessionFactory.Create() => CreateForQuery();

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
