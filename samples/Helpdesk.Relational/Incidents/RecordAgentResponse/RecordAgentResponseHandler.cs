using Helpdesk.Relational.Incidents.Errors;

namespace Helpdesk.Relational.Incidents.RecordAgentResponse;

public static class RecordAgentResponseHandler
{
    public static AgentRespondedToIncident Handle(Incident current, RecordAgentResponseToIncident command)
    {
        if (current.IsSatisfiedBy(IncidentSpecification.IsClosed))
            throw new IncidentAlreadyClosedException(command.IncidentId);

        return new AgentRespondedToIncident(command.IncidentId, command.Response, DateTimeOffset.UtcNow);
    }
}
