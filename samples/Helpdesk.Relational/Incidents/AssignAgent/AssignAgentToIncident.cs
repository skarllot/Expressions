namespace Helpdesk.Relational.Incidents.AssignAgent;

public record AssignAgentToIncident(
    Guid IncidentId,
    Guid AgentId
);
