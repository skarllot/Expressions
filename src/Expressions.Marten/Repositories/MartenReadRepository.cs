using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Repositories;

namespace Raiqub.Expressions.Marten.Repositories;

public class MartenReadRepository<T> : IReadRepository<T>
{
    private readonly ILogger<MartenReadRepository<T>> _logger;
    private readonly MartenSession _session;

    public MartenReadRepository(ILogger<MartenReadRepository<T>> logger, MartenSession session)
    {
        _logger = logger;
        _session = session;
    }

    public ISession Session => _session;

    public IQuery<TResult> Query<TResult>(QueryModel<T, TResult> queryModel, ChangeTracking? tracking = null)
    {
        return new MartenQuery<T, TResult>(
            _logger,
            _session,
            queryModel,
            tracking ?? ChangeTracking.Default);
    }
}
