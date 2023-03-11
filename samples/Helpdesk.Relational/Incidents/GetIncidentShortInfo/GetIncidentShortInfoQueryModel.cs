using Raiqub.Expressions;
using Raiqub.Expressions.Queries;

namespace Helpdesk.Relational.Incidents.GetIncidentShortInfo;

public class GetIncidentShortInfoQueryModel : QueryModel<Incident, IncidentShortInfo>
{
    public GetIncidentShortInfoQueryModel(Guid customerId, int pageNumber, int pageSize)
    {
        CustomerId = customerId;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public Guid CustomerId { get; }
    public int PageNumber { get; }
    public int PageSize { get; }

    protected override IEnumerable<Specification<Incident>> GetPreconditions()
    {
        yield return IncidentSpecification.OfCustomer(CustomerId);
    }

    protected override IQueryable<IncidentShortInfo> ExecuteCore(IQueryable<Incident> source) =>
        source
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .Select(
                incident => new IncidentShortInfo(
                    incident.Id,
                    incident.CustomerId,
                    incident.Status,
                    0,
                    incident.Category,
                    incident.Priority));
}
