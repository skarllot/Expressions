namespace Helpdesk.Relational.Incidents.Close;

public record IncidentClosed(
    Guid IncidentId,
    Guid ClosedBy,
    DateTimeOffset ClosedAt
);
