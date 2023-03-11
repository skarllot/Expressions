using Helpdesk.Relational.Incidents.Acknowledge;
using Helpdesk.Relational.Incidents.AssignAgent;
using Helpdesk.Relational.Incidents.Categorise;
using Helpdesk.Relational.Incidents.Close;
using Helpdesk.Relational.Incidents.LogNew;
using Helpdesk.Relational.Incidents.Prioritise;
using Helpdesk.Relational.Incidents.RecordAgentResponse;
using Helpdesk.Relational.Incidents.RecordCustomerResponse;
using Helpdesk.Relational.Incidents.Resolve;

namespace Helpdesk.Relational.Incidents;

public record Incident(
    Guid Id,
    Guid CustomerId,
    Contact Contact,
    string Description,
    Guid LoggedBy,
    DateTimeOffset LoggedAt,
    IncidentStatus Status = IncidentStatus.Pending,
    bool HasOutstandingResponseToCustomer = false,
    IncidentCategory? Category = null,
    Guid? CategorisedBy = null,
    DateTimeOffset? CategorisedAt = null,
    IncidentPriority? Priority = null,
    Guid? PrioritisedBy = null,
    DateTimeOffset? PrioritisedAt = null,
    Guid? AgentId = null,
    DateTimeOffset? AssignedAt = null,
    int Version = 1)
{
    public static Incident Create(IncidentLogged logged) =>
        new(logged.IncidentId, logged.CustomerId, logged.Contact, logged.Description, logged.LoggedBy, logged.LoggedAt);

    public Incident Apply(IncidentCategorised categorised) =>
        this with
        {
            Category = categorised.Category,
            CategorisedBy = categorised.CategorisedBy,
            CategorisedAt = categorised.CategorisedAt
        };

    public Incident Apply(IncidentPrioritised prioritised) =>
        this with
        {
            Priority = prioritised.Priority,
            PrioritisedBy = prioritised.PrioritisedBy,
            PrioritisedAt = prioritised.PrioritisedAt
        };

    public Incident Apply(AgentAssignedToIncident agentAssigned) =>
        this with { AgentId = agentAssigned.AgentId, AssignedAt = agentAssigned.AssignedAt };

    public Incident Apply(AgentRespondedToIncident agentResponded) =>
        this with { HasOutstandingResponseToCustomer = false };

    public Incident Apply(CustomerRespondedToIncident customerResponded) =>
        this with { HasOutstandingResponseToCustomer = true };

    public Incident Apply(IncidentResolved resolved) =>
        this with { Status = IncidentStatus.Resolved };

    public Incident Apply(ResolutionAcknowledgedByCustomer acknowledged) =>
        this with { Status = IncidentStatus.ResolutionAcknowledgedByCustomer };

    public Incident Apply(IncidentClosed closed) =>
        this with { Status = IncidentStatus.Closed };
}
