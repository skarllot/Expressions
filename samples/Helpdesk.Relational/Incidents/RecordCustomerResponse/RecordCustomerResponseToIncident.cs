namespace Helpdesk.Relational.Incidents.RecordCustomerResponse;

public record RecordCustomerResponseToIncident(
    Guid IncidentId,
    IncidentResponse.FromCustomer Response
);
