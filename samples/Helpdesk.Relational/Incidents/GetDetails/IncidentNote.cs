namespace Helpdesk.Relational.Incidents.GetDetails;

public record IncidentNote(
    IncidentNoteType Type,
    Guid From,
    string Content,
    bool VisibleToCustomer
);
