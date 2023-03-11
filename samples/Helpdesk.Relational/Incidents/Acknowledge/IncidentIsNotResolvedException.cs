namespace Helpdesk.Relational.Incidents.Acknowledge;

public sealed class IncidentIsNotResolvedException : Exception
{
    public IncidentIsNotResolvedException(Guid id)
        : base($"Only resolved incident can be acknowledged: {id}")
    {
    }
}
