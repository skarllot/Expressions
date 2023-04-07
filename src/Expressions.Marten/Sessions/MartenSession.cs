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

    public void Add<TEntity>(TEntity entity)
        where TEntity : class => AddRange(new[] { entity });

    public void AddRange<TEntity>(IEnumerable<TEntity> entities)
        where TEntity : class => _session.Store(entities);

    public ValueTask AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return AddRangeAsync(new[] { entity }, cancellationToken);
    }

    public ValueTask AddRangeAsync<TEntity>(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        _session.Store(entities);
        return default;
    }

    public void Remove<TEntity>(TEntity entity)
        where TEntity : class => _session.Delete(entity);

    public void RemoveRange<TEntity>(IEnumerable<TEntity> entities)
        where TEntity : class => _session.DeleteObjects(entities);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _session.SaveChangesAsync(cancellationToken);

    public void Update<TEntity>(TEntity entity)
        where TEntity : class => UpdateRange(new[] { entity });

    public void UpdateRange<TEntity>(IEnumerable<TEntity> entities)
        where TEntity : class => _session.Store(entities);
}
