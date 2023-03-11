using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.Repositories;

public static class RepositoryExtensions
{
    public static IQuery<T> Using<T>(
        this IReadRepository<T> repository,
        IQuerySession? session = null)
    {
        return repository.Query(QueryModel.Create<T>(), session);
    }

    public static IQuery<T> Using<T>(
        this IReadRepository<T> repository,
        Specification<T> specification,
        IQuerySession? session = null)
    {
        return repository.Query(QueryModel.Create(specification), session);
    }
}
