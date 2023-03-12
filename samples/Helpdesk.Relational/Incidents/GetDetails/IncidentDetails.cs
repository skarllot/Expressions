using Helpdesk.Relational.Incidents.Categorise;
using Helpdesk.Relational.Incidents.Prioritise;

namespace Helpdesk.Relational.Incidents.GetDetails;

public record IncidentDetails(
    Guid Id,
    Guid CustomerId,
    IncidentStatus Status,
    IncidentNote[] Notes,
    IncidentCategory? Category = null,
    IncidentPriority? Priority = null,
    Guid? AgentId = null
);
