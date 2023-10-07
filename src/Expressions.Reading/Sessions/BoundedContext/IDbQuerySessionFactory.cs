namespace Raiqub.Expressions.Sessions.BoundedContext;

/// <summary>Represents a factory for creating database query sessions for a bounded context.</summary>
public interface IDbQuerySessionFactory<out TContext>
{
    /// <summary>Creates a new database query session.</summary>
    /// <returns>A new database query session.</returns>
    IDbQuerySession<TContext> Create();
}
