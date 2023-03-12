using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.Repositories;

public interface IReadRepository<T> : IAsyncDisposable, IDisposable
{
    ISession Session { get; }

    IQuery<TResult> Query<TResult>(QueryModel<T, TResult> queryModel, ChangeTracking? tracking = null);
}
