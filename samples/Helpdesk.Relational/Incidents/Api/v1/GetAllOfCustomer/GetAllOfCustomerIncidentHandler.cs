using Helpdesk.Relational.Incidents.GetShortInfo;
using Raiqub.Expressions.Repositories;
using Raiqub.Expressions.Sessions;

namespace Helpdesk.Relational.Incidents.Api.v1.GetAllOfCustomer;

public class GetAllOfCustomerIncidentHandler
{
    private readonly ISessionFactory<IDefaultContext> _sessionFactory;
    private readonly IReadRepository<IDefaultContext, Incident> _incidentRepository;

    public GetAllOfCustomerIncidentHandler(
        ISessionFactory<IDefaultContext> sessionFactory,
        IReadRepository<IDefaultContext, Incident> incidentRepository)
    {
        _sessionFactory = sessionFactory;
        _incidentRepository = incidentRepository;
    }

    public async Task<IReadOnlyList<IncidentShortInfo>> Execute(
        GetAllOfCustomerIncidentRequest request,
        CancellationToken cancellationToken)
    {
        await using var session = _sessionFactory.CreateForQuery();

        var incidentShortInfos = await _incidentRepository
            .Query(
                new GetIncidentShortInfoQueryModel(request.CustomerId, request.PageNumber ?? 1, request.PageSize ?? 10),
                session)
            .ToListAsync(cancellationToken);

        return incidentShortInfos;
    }
}
