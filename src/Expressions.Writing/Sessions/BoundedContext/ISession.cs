namespace Raiqub.Expressions.Sessions.BoundedContext;

public interface ISession<out TContext> : IQuerySession<TContext>, ISession
{
}
