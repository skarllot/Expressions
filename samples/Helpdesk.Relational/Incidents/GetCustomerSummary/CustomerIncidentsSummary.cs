namespace Helpdesk.Relational.Incidents.GetCustomerSummary;

public record CustomerIncidentsSummary(Guid Id)
{
    public int Pending { get; init; }
    public int Resolved { get; init; }
    public int Acknowledged { get; init; }
    public int Closed { get; init; }
}
