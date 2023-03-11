namespace Helpdesk.Relational.Incidents.AssignAgent;

public record AgentAssignedToIncident(
    Guid IncidentId,
    Guid AgentId,
    DateTimeOffset AssignedAt
);
