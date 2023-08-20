using Raiqub.Expressions.Queries.Internal;

namespace Raiqub.Expressions.Queries;

public static class QueryModel
{
    public static IQueryModel<TResult> Create<TResult>(Func<IQuerySource, IQueryable<TResult>> queryModel) =>
        new AnonymousQueryModel<TResult>(queryModel);
}
