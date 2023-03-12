namespace Raiqub.Expressions.Repositories;

public interface ISession
{
    void Open(ChangeTracking? tracking = null);

    void OpenForQuery();

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
