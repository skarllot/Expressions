namespace Helpdesk.Relational.Incidents.Resolve;

public sealed class IncidentHasOutstandingResponsesWhenResolvingException : Exception
{
    public IncidentHasOutstandingResponsesWhenResolvingException(Guid id)
        : base($"Cannot resolve incident that has outstanding responses to customer: {id}")
    {
    }
}
