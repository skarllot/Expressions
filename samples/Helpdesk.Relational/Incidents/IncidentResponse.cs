namespace Helpdesk.Relational.Incidents;

public abstract record IncidentResponse
{
    public record FromAgent(
            Guid AgentId,
            string Content,
            bool VisibleToCustomer)
        : IncidentResponse(Content);

    public record FromCustomer(
            Guid CustomerId,
            string Content)
        : IncidentResponse(Content);

    private IncidentResponse(string content)
    {
        Content = content;
    }

    public string Content { get; init; }
}
