using Helpdesk.Relational.Incidents.GetShortInfo;
using Raiqub.Expressions.Sessions;

namespace Helpdesk.Relational.Incidents.Api.v1.GetAllOfCustomer;

public class GetAllOfCustomerIncidentHandler
{
    private readonly IQuerySessionFactory _sessionFactory;

    public GetAllOfCustomerIncidentHandler(IQuerySessionFactory sessionFactory)
    {
        _sessionFactory = sessionFactory;
    }

    public async Task<IReadOnlyList<IncidentShortInfo>> Execute(
        GetAllOfCustomerIncidentRequest request,
        CancellationToken cancellationToken)
    {
        await using var session = _sessionFactory.Create();

        var incidentShortInfos = await session
            .Query(
                new GetIncidentShortInfoQueryModel(request.CustomerId, request.PageNumber ?? 1, request.PageSize ?? 10))
            .ToListAsync(cancellationToken);

        return incidentShortInfos;
    }
}
