using Raiqub.Expressions;

namespace Helpdesk.Relational.Incidents;

public static class IncidentSpecification
{
    public static Specification<Incident> IsClosed { get; } =
        Specification.Create<Incident>(incident => incident.Status == IncidentStatus.Closed);

    public static Specification<Incident> IsResolved { get; } =
        Specification.Create<Incident>(incident => incident.Status == IncidentStatus.Resolved);

    public static Specification<Incident> IsNotResolved { get; } =
        !IsResolved;

    public static Specification<Incident> IsResolvedOrClosed { get; } =
        IsResolved | IsClosed;

    public static bool IsSatisfiedBy(this Incident incident, Specification<Incident> specification) =>
        specification.IsSatisfiedBy(incident);

    public static Specification<Incident> OfCustomer(Guid customerId) =>
        Specification.Create<Incident>(incident => incident.CustomerId == customerId);
}
