using Helpdesk.Relational.Incidents.Errors;

namespace Helpdesk.Relational.Incidents.RecordCustomerResponse;

public static class RecordCustomerResponseScenario
{
    public static CustomerRespondedToIncident Execute(Incident current, RecordCustomerResponseToIncident command)
    {
        if (current.IsSatisfiedBy(IncidentSpecification.IsClosed))
            throw new IncidentAlreadyClosedException(command.IncidentId);

        return new CustomerRespondedToIncident(command.IncidentId, command.Response, DateTimeOffset.UtcNow);
    }
}
