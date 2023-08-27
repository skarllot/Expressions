namespace Helpdesk.Relational.Incidents.Resolve;

public static class ResolveScenario
{
    public static IncidentResolved Execute(Incident current, ResolveIncident command)
    {
        if (current.IsSatisfiedBy(IncidentSpecification.IsResolvedOrClosed))
            throw new IncidentAlreadyResolvedOrClosedWhenResolvingException(command.IncidentId);

        if (current.HasOutstandingResponseToCustomer)
            throw new IncidentHasOutstandingResponsesWhenResolvingException(command.IncidentId);

        return new IncidentResolved(
            command.IncidentId,
            command.Resolution,
            command.ResolvedBy,
            DateTimeOffset.UtcNow);
    }
}
