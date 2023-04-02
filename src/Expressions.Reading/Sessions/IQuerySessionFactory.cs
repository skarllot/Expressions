namespace Raiqub.Expressions.Sessions;

/// <summary>Represents a factory for creating query sessions.</summary>
public interface IQuerySessionFactory
{
    /// <summary>Creates a new query session.</summary>
    /// <returns>A new query session.</returns>
    IQuerySession Create();
}
