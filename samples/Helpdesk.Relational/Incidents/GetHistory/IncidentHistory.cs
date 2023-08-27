namespace Helpdesk.Relational.Incidents.GetHistory;

public record IncidentHistory(
    Guid Id,
    Guid IncidentId,
    string Description
);
