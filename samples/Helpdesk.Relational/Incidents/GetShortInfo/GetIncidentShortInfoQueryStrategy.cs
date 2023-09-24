using Raiqub.Expressions;
using Raiqub.Expressions.Queries;

namespace Helpdesk.Relational.Incidents.GetShortInfo;

public class GetIncidentShortInfoQueryStrategy : EntityQueryStrategy<Incident, IncidentShortInfo>
{
    public GetIncidentShortInfoQueryStrategy(Guid customerId, int pageNumber, int pageSize)
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
                    incident.Responses.Count,
                    incident.Category,
                    incident.Priority));
}
