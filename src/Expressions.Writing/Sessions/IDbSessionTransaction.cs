namespace Raiqub.Expressions.Sessions;

public interface IDbSessionTransaction : IAsyncDisposable, IDisposable
{
    /// <summary>Commits all changes made to the database in the current transaction.</summary>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task CommitAsync(CancellationToken cancellationToken = default);
}
