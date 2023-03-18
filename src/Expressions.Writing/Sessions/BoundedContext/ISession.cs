namespace Raiqub.Expressions.Sessions.BoundedContext;

public interface ISession<out TContext> : IQuerySession<TContext>
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
