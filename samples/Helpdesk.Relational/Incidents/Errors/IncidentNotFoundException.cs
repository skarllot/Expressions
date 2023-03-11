namespace Helpdesk.Relational.Incidents.Errors;

public sealed class IncidentNotFoundException : Exception
{
    public IncidentNotFoundException(Guid id)
        : base($"No incident found with identity {id}")
    {
    }
}
