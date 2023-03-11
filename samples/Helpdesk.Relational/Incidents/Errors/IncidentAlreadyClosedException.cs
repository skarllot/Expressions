namespace Helpdesk.Relational.Incidents.Errors;

public sealed class IncidentAlreadyClosedException : Exception
{
    public IncidentAlreadyClosedException(Guid id)
        : base($"Incident is already closed: {id}")
    {
    }
}
