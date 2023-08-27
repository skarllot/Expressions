using Helpdesk.Relational.Incidents.Errors;

namespace Helpdesk.Relational.Incidents.RecordAgentResponse;

public static class RecordAgentResponseScenario
{
    public static AgentRespondedToIncident Execute(Incident current, RecordAgentResponseToIncident command)
    {
        if (current.IsSatisfiedBy(IncidentSpecification.IsClosed))
            throw new IncidentAlreadyClosedException(command.IncidentId);

        return new AgentRespondedToIncident(command.IncidentId, command.Response, DateTimeOffset.UtcNow);
    }
}
