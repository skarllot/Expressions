using Helpdesk.Relational.Incidents.Categorise;
using Helpdesk.Relational.Incidents.Prioritise;

namespace Helpdesk.Relational.Incidents.GetShortInfo;

public record IncidentShortInfo(
    Guid Id,
    Guid CustomerId,
    IncidentStatus Status,
    int NotesCount,
    IncidentCategory? Category = null,
    IncidentPriority? Priority = null
);
