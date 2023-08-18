namespace Raiqub.Expressions.Sessions.BoundedContext;

public interface IDbQuerySessionFactory<out TContext>
{
    IDbQuerySession<TContext> Create();
}
