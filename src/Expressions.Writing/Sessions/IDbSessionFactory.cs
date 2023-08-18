namespace Raiqub.Expressions.Sessions;

/// <summary>Represents a factory for creating database sessions for data access.</summary>
public interface IDbSessionFactory
{
    /// <summary>Creates a new database session.</summary>
    /// <param name="tracking">The change tracking mode of the new session.</param>
    /// <returns>A new database session.</returns>
    IDbSession Create(ChangeTracking? tracking = null);
}
