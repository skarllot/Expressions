using Raiqub.Expressions.Queries.Paging;
using Raiqub.Expressions.Sessions;

namespace Helpdesk.Relational.Incidents.GetShortInfo.v1;

public class GetIncidentShortInfoUseCase
{
    private readonly IDbQuerySession _dbQuerySession;

    public GetIncidentShortInfoUseCase(IDbQuerySession dbQuerySession)
    {
        _dbQuerySession = dbQuerySession;
    }

    public async Task<PagedResult<IncidentShortInfo>> Execute(Guid customerId, int? pageNumber, int? pageSize, CancellationToken cancellationToken)
    {
        return await _dbQuerySession
            .Query(new GetIncidentShortInfoQueryStrategy(customerId))
            .ToPagedListAsync(pageNumber ?? 1, pageSize ?? 10, cancellationToken);
    }
}
