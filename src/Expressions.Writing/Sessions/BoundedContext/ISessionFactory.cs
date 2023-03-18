namespace Raiqub.Expressions.Sessions.BoundedContext;

public interface ISessionFactory<out TContext>
{
    ISession<TContext> Create(ChangeTracking? tracking = null);
}
