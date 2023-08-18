namespace Raiqub.Expressions.Sessions.BoundedContext;

public interface IDbSession<out TContext> : IDbQuerySession<TContext>, IDbSession
{
}
