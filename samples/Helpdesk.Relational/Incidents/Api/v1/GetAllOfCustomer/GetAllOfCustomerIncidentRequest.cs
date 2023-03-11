namespace Helpdesk.Relational.Incidents.Api.v1.GetAllOfCustomer;

public record GetAllOfCustomerIncidentRequest(
    Guid CustomerId,
    int? PageNumber,
    int? PageSize);
