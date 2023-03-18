using Helpdesk.Relational.Incidents.GetDetails;
using Raiqub.Expressions.Sessions;

namespace Helpdesk.Relational.Incidents.Api.v1.GetDetails;

public class GetIncidentDetailsHandler
{
    private readonly IQuerySession _querySession;

    public GetIncidentDetailsHandler(IQuerySession querySession)
    {
        _querySession = querySession;
    }

    public async Task<IncidentDetails?> Execute(GetIncidentDetailsRequest request, CancellationToken cancellationToken)
    {
        IncidentDetails? result = await _querySession
            .Query(new GetIncidentDetailsQueryModel(request.IncidentId))
            .FirstOrDefaultAsync(cancellationToken);

        return result;
    }
}
