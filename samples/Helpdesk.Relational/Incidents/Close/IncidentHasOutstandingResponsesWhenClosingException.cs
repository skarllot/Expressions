namespace Helpdesk.Relational.Incidents.Close;

public sealed class IncidentHasOutstandingResponsesWhenClosingException : Exception
{
    public IncidentHasOutstandingResponsesWhenClosingException(Guid id)
        : base($"Cannot close incident that has outstanding responses to customer: {id}")
    {
    }
}
