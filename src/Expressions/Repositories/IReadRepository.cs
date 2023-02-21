using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.Repositories;

public interface IReadRepository<T>
    where T : class
{
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Specification<T> specification, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync<TResult>(Query<T, TResult> query, CancellationToken cancellationToken = default);

    Task<long> CountAsync(CancellationToken cancellationToken = default);
    Task<long> CountAsync(Specification<T> specification, CancellationToken cancellationToken = default);
    Task<long> CountAsync<TResult>(Query<T, TResult> query, CancellationToken cancellationToken = default);

    Task<T?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(ChangeTracking tracking, CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(Specification<T> specification, CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(Specification<T> specification, ChangeTracking tracking, CancellationToken cancellationToken = default);
    Task<TResult?> FirstOrDefaultAsync<TResult>(Query<T, TResult> query, CancellationToken cancellationToken = default);
    Task<TResult?> FirstOrDefaultAsync<TResult>(Query<T, TResult> query, ChangeTracking tracking, CancellationToken cancellationToken = default);

    Task<List<T>> ListAsync(CancellationToken cancellationToken = default);
    Task<List<T>> ListAsync(ChangeTracking tracking, CancellationToken cancellationToken = default);
    Task<List<T>> ListAsync(Specification<T> specification, CancellationToken cancellationToken = default);
    Task<List<T>> ListAsync(Specification<T> specification, ChangeTracking tracking, CancellationToken cancellationToken = default);
    Task<List<TResult>> ListAsync<TResult>(Query<T, TResult> query, CancellationToken cancellationToken = default);
    Task<List<TResult>> ListAsync<TResult>(Query<T, TResult> query, ChangeTracking tracking, CancellationToken cancellationToken = default);

    Task<T?> SingleOrDefaultAsync(CancellationToken cancellationToken = default);
    Task<T?> SingleOrDefaultAsync(ChangeTracking tracking, CancellationToken cancellationToken = default);
    Task<T?> SingleOrDefaultAsync(Specification<T> specification, CancellationToken cancellationToken = default);
    Task<T?> SingleOrDefaultAsync(Specification<T> specification, ChangeTracking tracking, CancellationToken cancellationToken = default);
    Task<TResult?> SingleOrDefaultAsync<TResult>(Query<T, TResult> query, CancellationToken cancellationToken = default);
    Task<TResult?> SingleOrDefaultAsync<TResult>(Query<T, TResult> query, ChangeTracking tracking, CancellationToken cancellationToken = default);
}
