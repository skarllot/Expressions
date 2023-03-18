using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.Sessions.BoundedContext;

public interface IQuerySession<out TContext> : IAsyncDisposable, IDisposable
{
    TContext Context { get; }

    ChangeTracking Tracking { get; }

    IQuery<TResult> Query<TEntity, TResult>(IQueryModel<TEntity, TResult> queryModel)
        where TEntity : class;
}
