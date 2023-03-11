namespace Helpdesk.Relational.Incidents.LogNew;

public static class LogHandler
{
    public static IncidentLogged Handle(LogIncident command)
    {
        return new IncidentLogged(
            command.IncidentId,
            command.CustomerId,
            command.Contact,
            command.Description,
            command.LoggedBy,
            DateTimeOffset.UtcNow);
    }
}
