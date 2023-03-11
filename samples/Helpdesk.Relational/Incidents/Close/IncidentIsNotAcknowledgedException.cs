namespace Helpdesk.Relational.Incidents.Close;

public sealed class IncidentIsNotAcknowledgedException : Exception
{
    public IncidentIsNotAcknowledgedException(Guid id)
        : base($"Only incident with acknowledged resolution can be closed: {id}")
    {
    }
}
