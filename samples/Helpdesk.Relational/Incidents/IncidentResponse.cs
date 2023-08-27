namespace Helpdesk.Relational.Incidents;

public abstract record IncidentResponse
{
    public record FromAgent(
            Guid AgentId,
            string Content,
            bool VisibleToCustomer,
            DateTimeOffset RespondedAt)
        : IncidentResponse(Content, RespondedAt);

    public record FromCustomer(
            Guid CustomerId,
            string Content,
            DateTimeOffset RespondedAt)
        : IncidentResponse(Content, RespondedAt);

    private IncidentResponse(string content, DateTimeOffset respondedAt)
    {
        Content = content;
        RespondedAt = respondedAt;
    }

    public string Content { get; init; }
    public DateTimeOffset RespondedAt { get; init; }
}
