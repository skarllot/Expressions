namespace Raiqub.Expressions.Sessions.BoundedContext;

/// <summary>Represents a factory for creating database sessions for data access in a bounded context.</summary>
/// <typeparam name="TContext">The type of the bounded context.</typeparam>
public interface IDbSessionFactory<out TContext>
{
    /// <summary>Creates a new database session.</summary>
    /// <param name="tracking">The change tracking mode of the new session.</param>
    /// <returns>A new database session.</returns>
    IDbSession<TContext> Create(ChangeTracking? tracking = null);
}
