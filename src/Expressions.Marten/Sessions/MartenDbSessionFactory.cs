using Marten;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Expressions.Marten.Sessions;

/// <summary>Marten-based implementation of a factory for creating database sessions for data access.</summary>
public class MartenDbSessionFactory : IDbSessionFactory, IDbQuerySessionFactory
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly IDocumentStore _documentStore;

    /// <summary>Initializes a new instance of the <see cref="MartenDbSessionFactory"/> class.</summary>
    /// <param name="loggerFactory">The factory used to create loggers.</param>
    /// <param name="documentStore">The <see cref="IDocumentSession"/> to read/write.</param>
    public MartenDbSessionFactory(ILoggerFactory loggerFactory, IDocumentStore documentStore)
    {
        _loggerFactory = loggerFactory;
        _documentStore = documentStore;
    }

    /// <summary>Creates a new database session.</summary>
    /// <param name="tracking">The change tracking mode of the new session.</param>
    /// <returns>A new database session.</returns>
    public IDbSession Create(ChangeTracking? tracking = null) => new MartenDbSession(
        _loggerFactory.CreateLogger<MartenDbSession>(),
        CreateSession(_documentStore, tracking ?? ChangeTracking.Default),
        tracking ?? ChangeTracking.Default);

    /// <summary>Creates a new database query session.</summary>
    /// <returns>A new database query session.</returns>
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
