using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Expressions.Repositories;

public interface IReadRepository<in TContext, TEntity>
{
    IQuery<TResult> Query<TResult>(
        QueryModel<TEntity, TResult> queryModel,
        IReadSession<TContext>? session = null);
}
