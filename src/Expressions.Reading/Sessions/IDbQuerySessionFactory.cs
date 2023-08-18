namespace Raiqub.Expressions.Sessions;

/// <summary>Represents a factory for creating database query sessions.</summary>
public interface IDbQuerySessionFactory
{
    /// <summary>Creates a new database query session.</summary>
    /// <returns>A new database query session.</returns>
    IDbQuerySession Create();
}
