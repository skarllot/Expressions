namespace Raiqub.Expressions.Sessions.BoundedContext;

/// <summary>Represents a session for querying data from database of a bounded context.</summary>
/// <typeparam name="TContext">The type of the bounded context.</typeparam>
public interface IDbQuerySession<out TContext> : IDbQuerySession
{
    /// <summary>Gets the bounded context associated with this query session.</summary>
    /// <remarks>
    /// A bounded context is a clear boundary within which a domain model is defined and used.
    /// It is responsible for defining a ubiquitous language, which is a shared set of terms and concepts
    /// used by all members of the project team and domain experts to communicate about the domain.
    /// </remarks>
    TContext Context { get; }
}
