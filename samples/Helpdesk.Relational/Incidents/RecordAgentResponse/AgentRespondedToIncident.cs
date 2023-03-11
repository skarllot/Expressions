namespace Helpdesk.Relational.Incidents.RecordAgentResponse;

public record AgentRespondedToIncident(
    Guid IncidentId,
    IncidentResponse.FromAgent Response,
    DateTimeOffset RespondedAt
);
