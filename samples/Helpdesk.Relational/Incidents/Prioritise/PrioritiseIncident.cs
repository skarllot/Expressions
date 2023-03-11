namespace Helpdesk.Relational.Incidents.Prioritise;

public record PrioritiseIncident(
    Guid IncidentId,
    IncidentPriority Priority,
    Guid PrioritisedBy
);
