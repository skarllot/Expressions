using Raiqub.Expressions;
using Raiqub.Expressions.Queries;

namespace Helpdesk.Relational.Incidents.GetShortInfo;

public class GetIncidentShortInfoQueryStrategy : EntityQueryStrategy<Incident, IncidentShortInfo>
{
    public GetIncidentShortInfoQueryStrategy(Guid customerId)
    {
        CustomerId = customerId;
    }

    public Guid CustomerId { get; }

    protected override IEnumerable<Specification<Incident>> GetPreconditions()
    {
        yield return IncidentSpecification.OfCustomer(CustomerId);
    }

    protected override IQueryable<IncidentShortInfo> ExecuteCore(IQueryable<Incident> source) =>
        source.Select(
            incident => new IncidentShortInfo(
                incident.Id,
                incident.CustomerId,
                incident.Status,
                incident.Responses.Count,
                incident.Category,
                incident.Priority));
}
