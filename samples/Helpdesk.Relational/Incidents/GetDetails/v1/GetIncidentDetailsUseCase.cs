using Raiqub.Expressions.Sessions;

namespace Helpdesk.Relational.Incidents.GetDetails.v1;

public class GetIncidentDetailsUseCase
{
    private readonly IDbQuerySession _dbQuerySession;

    public GetIncidentDetailsUseCase(IDbQuerySession dbQuerySession)
    {
        _dbQuerySession = dbQuerySession;
    }

    public async Task<IncidentDetails?> Execute(GetIncidentDetailsRequest request, CancellationToken cancellationToken)
    {
        IncidentDetails? result = await _dbQuerySession
            .Query(new GetIncidentDetailsQueryStrategy(request.IncidentId))
            .FirstOrDefaultAsync(cancellationToken);

        return result;
    }
}
