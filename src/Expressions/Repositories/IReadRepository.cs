using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.Repositories;

public interface IReadRepository<T>
{
    IQuery<TResult> Using<TResult>(QueryModel<T, TResult> queryModel, ChangeTracking? tracking = null);
}
