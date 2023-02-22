using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.Repositories;

public static class RepositoryExtensions
{
    public static IQuery<T> Using<T>(
        this IReadRepository<T> repository,
        ChangeTracking? tracking = null)
    {
        return repository.Using(QueryModel.Create<T>(), tracking);
    }

    public static IQuery<T> Using<T>(
        this IReadRepository<T> repository,
        Specification<T> specification,
        ChangeTracking? tracking = null)
    {
        return repository.Using(QueryModel.Create(specification), tracking);
    }
}
