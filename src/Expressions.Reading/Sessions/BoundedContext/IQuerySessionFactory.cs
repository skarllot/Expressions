namespace Raiqub.Expressions.Sessions.BoundedContext;

public interface IQuerySessionFactory<out TContext>
{
    IQuerySession<TContext> Create();
}
