using Raiqub.Expressions;
using Raiqub.Expressions.Queries;

namespace Helpdesk.Relational.Incidents.GetDetails;

public class GetIncidentDetailsQueryStrategy : EntityQueryStrategy<Incident, IncidentDetails>
{
    public GetIncidentDetailsQueryStrategy(Guid incidentId) => IncidentId = incidentId;

    public Guid IncidentId { get; }

    protected override IEnumerable<Specification<Incident>> GetPreconditions()
    {
        yield return IncidentSpecification.HasId(IncidentId);
    }

    protected override IQueryable<IncidentDetails> ExecuteCore(IQueryable<Incident> source) => source
        .Select(
            incident => new IncidentDetails(
                incident.Id,
                incident.CustomerId,
                incident.Status,
                incident.Responses
                    .Select(
                        r => new IncidentNote(
                            r is IncidentResponse.FromAgent
                                ? IncidentNoteType.FromAgent
                                : IncidentNoteType.FromCustomer,
                            r is IncidentResponse.FromAgent
                                ? ((IncidentResponse.FromAgent)r).AgentId
                                : ((IncidentResponse.FromCustomer)r).CustomerId,
                            r.Content,
                            !(r is IncidentResponse.FromAgent) || ((IncidentResponse.FromAgent)r).VisibleToCustomer))
                    .ToArray(),
                incident.Category,
                incident.Priority,
                incident.AgentId));
}
