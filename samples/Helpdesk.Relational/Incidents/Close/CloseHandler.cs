namespace Helpdesk.Relational.Incidents.Close;

public static class CloseHandler
{
    public static IncidentClosed Handle(Incident current, CloseIncident command)
    {
        if (current.Status is not IncidentStatus.ResolutionAcknowledgedByCustomer)
            throw new IncidentIsNotAcknowledgedException(command.IncidentId);

        if (current.HasOutstandingResponseToCustomer)
            throw new IncidentHasOutstandingResponsesWhenClosingException(command.IncidentId);

        return new IncidentClosed(command.IncidentId, command.ClosedBy, DateTimeOffset.UtcNow);
    }
}
