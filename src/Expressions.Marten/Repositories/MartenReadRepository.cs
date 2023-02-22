using Marten;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Repositories;

namespace Raiqub.Expressions.Marten.Repositories;

public class MartenReadRepository<T> : IReadRepository<T>
{
    private readonly ILogger<MartenReadRepository<T>> _logger;
    private readonly IDocumentStore _documentStore;

    public MartenReadRepository(ILogger<MartenReadRepository<T>> logger, IDocumentStore documentStore)
    {
        _logger = logger;
        _documentStore = documentStore;
    }

    public IQuery<TResult> Using<TResult>(QueryModel<T, TResult> queryModel, ChangeTracking? tracking = null)
    {
        return new MartenQuery<T, TResult>(
            _logger,
            _documentStore,
            queryModel,
            tracking ?? queryModel.DefaultChangeTracking ?? ChangeTracking.Default);
    }
}
