using Helpdesk.Relational.Incidents.GetDetails;
using Raiqub.Expressions.Repositories;
using Raiqub.Expressions.Sessions;

namespace Helpdesk.Relational.Incidents.Api.v1.GetDetails;

public class GetIncidentDetailsHandler
{
    private readonly IReadRepository<IDefaultContext, Incident> _incidentRepository;

    public GetIncidentDetailsHandler(IReadRepository<IDefaultContext, Incident> incidentRepository)
    {
        _incidentRepository = incidentRepository;
    }

    public async Task<IncidentDetails?> Execute(GetIncidentDetailsRequest request, CancellationToken cancellationToken)
    {
        IncidentDetails? result = await _incidentRepository
            .Query(new GetIncidentDetailsQueryModel(request.IncidentId))
            .FirstOrDefaultAsync(cancellationToken);

        return result;
    }
}
