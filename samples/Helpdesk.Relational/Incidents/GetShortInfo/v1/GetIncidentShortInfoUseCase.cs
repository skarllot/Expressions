using Raiqub.Expressions.Sessions;

namespace Helpdesk.Relational.Incidents.GetShortInfo.v1;

public class GetIncidentShortInfoUseCase
{
    private readonly IDbQuerySession _dbQuerySession;

    public GetIncidentShortInfoUseCase(IDbQuerySession dbQuerySession)
    {
        _dbQuerySession = dbQuerySession;
    }

    public async Task<IReadOnlyList<IncidentShortInfo>> Execute(Guid customerId, int? pageNumber, int? pageSize, CancellationToken cancellationToken)
    {
        return await _dbQuerySession
            .Query(new GetIncidentShortInfoQueryModel(customerId, pageNumber ?? 1, pageSize ?? 10))
            .ToListAsync(cancellationToken);
    }
}
