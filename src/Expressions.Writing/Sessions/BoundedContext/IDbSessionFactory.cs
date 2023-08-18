namespace Raiqub.Expressions.Sessions.BoundedContext;

public interface IDbSessionFactory<out TContext>
{
    IDbSession<TContext> Create(ChangeTracking? tracking = null);
}
