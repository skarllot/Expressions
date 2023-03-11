using Helpdesk.Relational.Incidents.Errors;

namespace Helpdesk.Relational.Incidents.RecordCustomerResponse;

public static class RecordCustomerResponseHandler
{
    public static CustomerRespondedToIncident Handle(Incident current, RecordCustomerResponseToIncident command)
    {
        if (current.IsSatisfiedBy(IncidentSpecification.IsClosed))
            throw new IncidentAlreadyClosedException(command.IncidentId);

        return new CustomerRespondedToIncident(command.IncidentId, command.Response, DateTimeOffset.UtcNow);
    }
}
