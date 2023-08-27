using Helpdesk.Relational.Incidents.Errors;

namespace Helpdesk.Relational.Incidents.Categorise;

public static class CategoriseScenario
{
    public static IncidentCategorised Execute(Incident current, CategoriseIncident command)
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
