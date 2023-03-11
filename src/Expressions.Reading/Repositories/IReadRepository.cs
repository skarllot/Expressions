using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.Repositories;

public interface IReadRepository<T>
{
    IQuery<TResult> Query<TResult>(QueryModel<T, TResult> queryModel, IQuerySession? session = null);
}
