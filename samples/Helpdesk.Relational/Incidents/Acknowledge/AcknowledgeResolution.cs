namespace Helpdesk.Relational.Incidents.Acknowledge;

public record AcknowledgeResolution(
    Guid IncidentId,
    Guid AcknowledgedBy
);
