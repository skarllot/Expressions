namespace Helpdesk.Relational.Incidents.RecordCustomerResponse;

public record CustomerRespondedToIncident(
    Guid IncidentId,
    IncidentResponse.FromCustomer Response,
    DateTimeOffset RespondedAt
);
