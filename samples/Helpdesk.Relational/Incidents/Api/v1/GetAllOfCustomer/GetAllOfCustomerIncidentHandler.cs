using Helpdesk.Relational.Incidents.GetShortInfo;
using Raiqub.Expressions.Repositories;

namespace Helpdesk.Relational.Incidents.Api.v1.GetAllOfCustomer;

public class GetAllOfCustomerIncidentHandler
{
    private readonly IReadRepository<Incident> _incidentRepository;

    public GetAllOfCustomerIncidentHandler(IReadRepository<Incident> incidentRepository)
    {
        _incidentRepository = incidentRepository;
    }

    public async Task<IReadOnlyList<IncidentShortInfo>> Execute(
        GetAllOfCustomerIncidentRequest request,
        CancellationToken cancellationToken)
    {
        _incidentRepository.Session.OpenForQuery();

        var incidentShortInfos = await _incidentRepository
            .Query(
                new GetIncidentShortInfoQueryModel(request.CustomerId, request.PageNumber ?? 1, request.PageSize ?? 10))
            .ToListAsync(cancellationToken);

        return incidentShortInfos;
    }
}
