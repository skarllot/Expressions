namespace Raiqub.Expressions.Sessions.BoundedContext;

public interface IQuerySession<out TContext> : IQuerySession
{
    /// <summary>Gets the bounded context associated with this query session.</summary>
    /// <remarks>
    /// A bounded context is a clear boundary within which a domain model is defined and used.
    /// It is responsible for defining a ubiquitous language, which is a shared set of terms and concepts
    /// used by all members of the project team and domain experts to communicate about the domain.
    /// </remarks>
    TContext Context { get; }
}
