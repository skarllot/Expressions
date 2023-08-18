using Helpdesk.Relational.Incidents.GetDetails;
using Raiqub.Expressions.Sessions;

namespace Helpdesk.Relational.Incidents.Api.v1.GetDetails;

public class GetIncidentDetailsHandler
{
    private readonly IDbQuerySession _dbQuerySession;

    public GetIncidentDetailsHandler(IDbQuerySession dbQuerySession)
    {
        _dbQuerySession = dbQuerySession;
    }

    public async Task<IncidentDetails?> Execute(GetIncidentDetailsRequest request, CancellationToken cancellationToken)
    {
        IncidentDetails? result = await _dbQuerySession
            .Query(new GetIncidentDetailsQueryModel(request.IncidentId))
            .FirstOrDefaultAsync(cancellationToken);

        return result;
    }
}
