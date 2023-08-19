namespace Raiqub.Expressions.Sessions;

/// <summary>Represents a session used to perform data access operations.</summary>
public interface IDbSession : IDbQuerySession
{
    /// <summary>Gets the change tracking mode for this session.</summary>
    /// <remarks>
    /// The change tracking mode determines how the session's change tracker will handle returned entities.
    /// </remarks>
    ChangeTracking Tracking { get; }

    /// <summary>Tracks the specified entity as added.</summary>
    /// <typeparam name="TEntity">The type of entity to add.</typeparam>
    /// <param name="entity">The entity to add.</param>
    /// <exception cref="ArgumentNullException"><paramref name="entity"/> is null.</exception>
    void Add<TEntity>(TEntity entity)
        where TEntity : class;

    /// <summary>Tracks the specified entities as added.</summary>
    /// <typeparam name="TEntity">The type of the entities to add.</typeparam>
    /// <param name="entities">The entities to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="entities"/> is null.</exception>
    void AddRange<TEntity>(IEnumerable<TEntity> entities)
        where TEntity : class;

    /// <summary>Tracks the specified entity as added.</summary>
    /// <typeparam name="TEntity">The type of entity to add.</typeparam>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="entity"/> is null.</exception>
    ValueTask AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class;

    /// <summary>Tracks the specified entities as added.</summary>
    /// <typeparam name="TEntity">The type of the entities to add.</typeparam>
    /// <param name="entities">The entities to add.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="entities"/> is null.</exception>
    ValueTask AddRangeAsync<TEntity>(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
        where TEntity : class;

    /// <summary>Tracks the specified entity as removed.</summary>
    /// <typeparam name="TEntity">The type of entity to remove.</typeparam>
    /// <param name="entity">The entity to remove.</param>
    /// <exception cref="ArgumentNullException"><paramref name="entity"/> is null.</exception>
    void Remove<TEntity>(TEntity entity)
        where TEntity : class;

    /// <summary>Tracks the specified entities as removed.</summary>
    /// <typeparam name="TEntity">The type of the entities to remove.</typeparam>
    /// <param name="entities">The entities to remove.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="entities"/> is null.</exception>
    void RemoveRange<TEntity>(IEnumerable<TEntity> entities)
        where TEntity : class;

    /// <summary>Asynchronously saves all changes made in this session to the underlying data store.</summary>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>Tracks the specified entity as modified.</summary>
    /// <typeparam name="TEntity">The type of entity to update.</typeparam>
    /// <param name="entity">The entity to update.</param>
    /// <exception cref="ArgumentNullException"><paramref name="entity"/> is null.</exception>
    void Update<TEntity>(TEntity entity)
        where TEntity : class;

    /// <summary>Tracks the specified entities as modified.</summary>
    /// <typeparam name="TEntity">The type of the entities to update.</typeparam>
    /// <param name="entities">The entities to update.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="entities"/> is null.</exception>
    void UpdateRange<TEntity>(IEnumerable<TEntity> entities)
        where TEntity : class;
}
