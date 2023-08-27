namespace Helpdesk.Relational.Incidents.LogNew.v1;

public sealed record LogIncidentRequest(
    Contact Contact,
    string Description);
