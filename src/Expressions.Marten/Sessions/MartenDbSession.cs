using System.Diagnostics.CodeAnalysis;
using Marten;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Expressions.Marten.Sessions;

/// <summary>Marten-based implementation of a database session for querying and saving instances.</summary>
public class MartenDbSession : MartenDbQuerySession, IDbSession
{
    private readonly IDocumentSession _session;

    /// <summary>Initializes a new instance of the <see cref="MartenDbSession"/> class.</summary>
    /// <param name="logger">The <see cref="ILogger"/> to log to.</param>
    /// <param name="session">The <see cref="IDocumentSession"/> to read/write.</param>
    /// <param name="tracking">The change tracking mode of the session.</param>
    public MartenDbSession(ILogger<MartenDbSession> logger, IDocumentSession session, ChangeTracking tracking)
        : base(logger, session)
    {
        _session = session;
        Tracking = tracking;
    }

    /// <inheritdoc />
    public override IDocumentSession MartenSession => _session;

    /// <inheritdoc />
    public ChangeTracking Tracking { get; }

    /// <inheritdoc />
    [DoesNotReturn]
    public ValueTask<IDbSessionTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException(
            "Transactions are not currently supported by Marten implementation of IDbSession");
    }

    /// <inheritdoc />
    public void Add<TEntity>(TEntity entity)
        where TEntity : class => AddRange(new[] { entity });

    /// <inheritdoc />
    public void AddRange<TEntity>(IEnumerable<TEntity> entities)
        where TEntity : class => _session.Store(entities);

    /// <inheritdoc />
    public ValueTask AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return AddRangeAsync(new[] { entity }, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask AddRangeAsync<TEntity>(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        _session.Store(entities);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public void Remove<TEntity>(TEntity entity)
        where TEntity : class => _session.Delete(entity);

    /// <inheritdoc />
    public void RemoveRange<TEntity>(IEnumerable<TEntity> entities)
        where TEntity : class => _session.DeleteObjects(entities);

    /// <inheritdoc />
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _session.SaveChangesAsync(cancellationToken);

    /// <inheritdoc />
    public void Update<TEntity>(TEntity entity)
        where TEntity : class => UpdateRange(new[] { entity });

    /// <inheritdoc />
    public void UpdateRange<TEntity>(IEnumerable<TEntity> entities)
        where TEntity : class => _session.Store(entities);
}
