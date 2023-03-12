using Helpdesk.Relational.Incidents.GetShortInfo;
using Raiqub.Expressions.Repositories;

namespace Helpdesk.Relational.Incidents.Api.v1.GetAllOfCustomer;

public class GetAllOfCustomerIncidentHandler
{
    private readonly ISessionFactory _sessionFactory;
    private readonly IReadRepository<Incident> _incidentRepository;

    public GetAllOfCustomerIncidentHandler(ISessionFactory sessionFactory, IReadRepository<Incident> incidentRepository)
    {
        _sessionFactory = sessionFactory;
        _incidentRepository = incidentRepository;
    }

    public async Task<IReadOnlyList<IncidentShortInfo>> Execute(
        GetAllOfCustomerIncidentRequest request,
        CancellationToken cancellationToken)
    {
        await using IQuerySession session = _sessionFactory.CreateForQuery();

        var incidentShortInfos = await _incidentRepository
            .Query(
                new GetIncidentShortInfoQueryModel(request.CustomerId, request.PageNumber ?? 1, request.PageSize ?? 10),
                session)
            .ToListAsync(cancellationToken);

        return incidentShortInfos;
    }
}
