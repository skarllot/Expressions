namespace Helpdesk.Relational.Incidents.Resolve;

public record ResolveIncident(
    Guid IncidentId,
    ResolutionType Resolution,
    Guid ResolvedBy
);
