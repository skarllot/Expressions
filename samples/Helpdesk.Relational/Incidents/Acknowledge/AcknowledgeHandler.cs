namespace Helpdesk.Relational.Incidents.Acknowledge;

public static class AcknowledgeHandler
{
    public static ResolutionAcknowledgedByCustomer Handle(Incident current, AcknowledgeResolution command)
    {
        if (current.IsSatisfiedBy(IncidentSpecification.IsNotResolved))
            throw new IncidentIsNotResolvedException(command.IncidentId);

        return new ResolutionAcknowledgedByCustomer(
            command.IncidentId,
            command.AcknowledgedBy,
            DateTimeOffset.UtcNow);
    }
}
