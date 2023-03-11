namespace Raiqub.Expressions.Repositories;

public interface IQuery<T>
{
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
    Task<long> CountAsync(CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> ToListAsync(CancellationToken cancellationToken = default);
    Task<T?> SingleOrDefaultAsync(CancellationToken cancellationToken = default);
}
