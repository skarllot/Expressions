using Helpdesk.Relational.Incidents.Errors;

namespace Helpdesk.Relational.Incidents.AssignAgent;

public static class AssignAgentScenario
{
    public static AgentAssignedToIncident Execute(Incident current, AssignAgentToIncident command)
    {
        if (current.IsSatisfiedBy(IncidentSpecification.IsClosed))
            throw new IncidentAlreadyClosedException(command.IncidentId);

        return new AgentAssignedToIncident(
            command.IncidentId,
            command.AgentId,
            DateTimeOffset.UtcNow);
    }
}
