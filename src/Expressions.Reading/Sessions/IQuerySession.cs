using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.Sessions;

public interface IQuerySession : IAsyncDisposable, IDisposable
{
    ChangeTracking Tracking { get; }

    IQuery<TResult> Query<TEntity, TResult>(IQueryModel<TEntity, TResult> queryModel)
        where TEntity : class;
}
