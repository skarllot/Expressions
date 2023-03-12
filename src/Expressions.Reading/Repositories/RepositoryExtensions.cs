using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.Repositories;

public static class RepositoryExtensions
{
    public static IQuery<T> Query<T>(
        this IReadRepository<T> repository,
        ChangeTracking? tracking = null)
    {
        return repository.Query(QueryModel.Create<T>(), tracking);
    }

    public static IQuery<T> Query<T>(
        this IReadRepository<T> repository,
        Specification<T> specification,
        ChangeTracking? tracking = null)
    {
        return repository.Query(QueryModel.Create(specification), tracking);
    }
}
