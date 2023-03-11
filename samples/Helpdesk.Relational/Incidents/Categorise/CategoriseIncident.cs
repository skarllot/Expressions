namespace Helpdesk.Relational.Incidents.Categorise;

public record CategoriseIncident(
    Guid IncidentId,
    IncidentCategory Category,
    Guid CategorisedBy
);
