namespace Raiqub.Expressions.Sessions;

public interface ISession<out TContext> : IReadSession<TContext>
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
