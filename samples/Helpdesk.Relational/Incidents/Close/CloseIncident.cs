namespace Helpdesk.Relational.Incidents.Close;

public record CloseIncident(
    Guid IncidentId,
    Guid ClosedBy
);
