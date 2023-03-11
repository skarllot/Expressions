namespace Raiqub.Expressions.Repositories;

public interface ISession : IQuerySession
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
