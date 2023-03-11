using Helpdesk.Relational.Incidents.Errors;

namespace Helpdesk.Relational.Incidents.Categorise;

public static class CategoriseHandler
{
    public static IncidentCategorised Handle(Incident current, CategoriseIncident command)
    {
        if (current.IsSatisfiedBy(IncidentSpecification.IsClosed))
            throw new IncidentAlreadyClosedException(command.IncidentId);

        return new IncidentCategorised(
            command.IncidentId,
            command.Category,
            command.CategorisedBy,
            DateTimeOffset.UtcNow);
    }
}
