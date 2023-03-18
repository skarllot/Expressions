namespace Raiqub.Expressions.Sessions;

public interface ISession : IQuerySession
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
