namespace Raiqub.Expressions.Sessions;

public interface ISessionFactory<out TContext>
{
    ISession<TContext> Create(ChangeTracking? tracking = null);

    IReadSession<TContext> CreateForQuery();
}
