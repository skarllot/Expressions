using Helpdesk.Relational.Incidents.Errors;

namespace Helpdesk.Relational.Incidents.Prioritise;

public static class PrioritiseScenario
{
    public static IncidentPrioritised Execute(Incident current, PrioritiseIncident command)
    {
        if (current.IsSatisfiedBy(IncidentSpecification.IsClosed))
            throw new IncidentAlreadyClosedException(command.IncidentId);

        return new IncidentPrioritised(
            command.IncidentId,
            command.Priority,
            command.PrioritisedBy,
            DateTimeOffset.UtcNow);
    }
}
