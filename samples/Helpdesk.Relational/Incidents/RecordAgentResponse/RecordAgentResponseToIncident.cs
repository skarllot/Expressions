namespace Helpdesk.Relational.Incidents.RecordAgentResponse;

public record RecordAgentResponseToIncident(
    Guid IncidentId,
    IncidentResponse.FromAgent Response
);
