namespace Helpdesk.Relational.Incidents.Resolve;

public static class ResolveHandler
{
    public static IncidentResolved Handle(Incident current, ResolveIncident command)
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
