namespace Helpdesk.Relational.Incidents.Resolve;

public sealed class IncidentAlreadyResolvedOrClosedWhenResolvingException : Exception
{
    public IncidentAlreadyResolvedOrClosedWhenResolvingException(Guid id)
        : base($"Cannot resolve already resolved or closed incident: {id}")
    {
    }
}
