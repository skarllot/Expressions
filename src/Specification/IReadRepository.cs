namespace Raiqub.Specification;

public interface IReadRepository<T>
    where T : class
{
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Specification<T> specification, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync<TResult>(Query<T, TResult> query, CancellationToken cancellationToken = default);

    Task<long> CountAsync(CancellationToken cancellationToken = default);
    Task<long> CountAsync(Specification<T> specification, CancellationToken cancellationToken = default);
    Task<long> CountAsync<TResult>(Query<T, TResult> query, CancellationToken cancellationToken = default);

    Task<T?> FirstOrDefaultAsync(CancellationToken cancellationToken);
    Task<T?> FirstOrDefaultAsync(ChangeTracking tracking = ChangeTracking.Default, CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(Specification<T> specification, CancellationToken cancellationToken);
    Task<T?> FirstOrDefaultAsync(Specification<T> specification, ChangeTracking tracking = ChangeTracking.Default, CancellationToken cancellationToken = default);
    Task<TResult?> FirstOrDefaultAsync<TResult>(Query<T, TResult> query, CancellationToken cancellationToken);
    Task<TResult?> FirstOrDefaultAsync<TResult>(Query<T, TResult> query, ChangeTracking tracking = ChangeTracking.Default, CancellationToken cancellationToken = default);

    Task<List<T>> ListAsync(CancellationToken cancellationToken);
    Task<List<T>> ListAsync(ChangeTracking tracking = ChangeTracking.Default, CancellationToken cancellationToken = default);
    Task<List<T>> ListAsync(Specification<T> specification, CancellationToken cancellationToken);
    Task<List<T>> ListAsync(Specification<T> specification, ChangeTracking tracking = ChangeTracking.Default, CancellationToken cancellationToken = default);
    Task<List<TResult>> ListAsync<TResult>(Query<T, TResult> query, CancellationToken cancellationToken);
    Task<List<TResult>> ListAsync<TResult>(Query<T, TResult> query, ChangeTracking tracking = ChangeTracking.Default, CancellationToken cancellationToken = default);

    Task<T?> SingleOrDefaultAsync(CancellationToken cancellationToken);
    Task<T?> SingleOrDefaultAsync(ChangeTracking tracking = ChangeTracking.Default, CancellationToken cancellationToken = default);
    Task<T?> SingleOrDefaultAsync(Specification<T> specification, CancellationToken cancellationToken);
    Task<T?> SingleOrDefaultAsync(Specification<T> specification, ChangeTracking tracking = ChangeTracking.Default, CancellationToken cancellationToken = default);
    Task<TResult?> SingleOrDefaultAsync<TResult>(Query<T, TResult> query, CancellationToken cancellationToken);
    Task<TResult?> SingleOrDefaultAsync<TResult>(Query<T, TResult> query, ChangeTracking tracking = ChangeTracking.Default, CancellationToken cancellationToken = default);
}
