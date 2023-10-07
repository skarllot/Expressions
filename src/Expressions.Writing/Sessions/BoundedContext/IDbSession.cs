namespace Raiqub.Expressions.Sessions.BoundedContext;

/// <summary>Represents a session used to perform data access operations for a bounded context.</summary>
/// <typeparam name="TContext">The type of the bounded context.</typeparam>
public interface IDbSession<out TContext> : IDbQuerySession<TContext>, IDbSession
{
}
