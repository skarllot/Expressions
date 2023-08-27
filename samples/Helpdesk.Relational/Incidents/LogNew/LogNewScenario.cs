namespace Helpdesk.Relational.Incidents.LogNew;

public static class LogNewScenario
{
    public static IncidentLogged Execute(LogIncident command)
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
