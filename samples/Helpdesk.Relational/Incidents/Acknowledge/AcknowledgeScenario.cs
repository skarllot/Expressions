namespace Helpdesk.Relational.Incidents.Acknowledge;

public static class AcknowledgeScenario
{
    public static ResolutionAcknowledgedByCustomer Execute(Incident current, AcknowledgeResolution command)
    {
        if (current.IsSatisfiedBy(IncidentSpecification.IsNotResolved))
            throw new IncidentIsNotResolvedException(command.IncidentId);

        return new ResolutionAcknowledgedByCustomer(
            command.IncidentId,
            command.AcknowledgedBy,
            DateTimeOffset.UtcNow);
    }
}
