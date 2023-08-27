using Helpdesk.Relational.Incidents.Acknowledge;
using Helpdesk.Relational.Incidents.AssignAgent;
using Helpdesk.Relational.Incidents.Categorise;
using Helpdesk.Relational.Incidents.Close;
using Helpdesk.Relational.Incidents.LogNew;
using Helpdesk.Relational.Incidents.Prioritise;
using Helpdesk.Relational.Incidents.RecordAgentResponse;
using Helpdesk.Relational.Incidents.RecordCustomerResponse;
using Helpdesk.Relational.Incidents.Resolve;

namespace Helpdesk.Relational.Incidents;

public class Incident
{
    private readonly List<IncidentResponse> _responses;

    private Incident(
        Guid id,
        Guid customerId,
        Contact contact,
        string description,
        Guid loggedBy,
        DateTimeOffset loggedAt)
        : this(id, customerId, description, loggedBy, loggedAt)
    {
        Contact = contact;
    }

    private Incident(
        Guid id,
        Guid customerId,
        string description,
        Guid loggedBy,
        DateTimeOffset loggedAt)
    {
        Id = id;
        CustomerId = customerId;
        Contact = null!;
        Description = description;
        LoggedBy = loggedBy;
        LoggedAt = loggedAt;
        Status = IncidentStatus.Pending;
        HasOutstandingResponseToCustomer = false;
        _responses = new List<IncidentResponse>();
    }

    public Guid Id { get; }
    public Guid CustomerId { get; }
    public Contact Contact { get; }
    public string Description { get; }
    public Guid LoggedBy { get; }
    public DateTimeOffset LoggedAt { get; }
    public IncidentStatus Status { get; private set; }
    public bool HasOutstandingResponseToCustomer { get; private set; }
    public IncidentCategory? Category { get; private set; }
    public Guid? CategorisedBy { get; private set; }
    public DateTimeOffset? CategorisedAt { get; private set; }
    public IncidentPriority? Priority { get; private set; }
    public Guid? PrioritisedBy { get; private set; }
    public DateTimeOffset? PrioritisedAt { get; private set; }
    public Guid? AgentId { get; private set; }
    public DateTimeOffset? AssignedAt { get; private set; }

    public IReadOnlyList<IncidentResponse> Responses => _responses;

    public static Incident Create(IncidentLogged logged) =>
        new(logged.IncidentId, logged.CustomerId, logged.Contact, logged.Description, logged.LoggedBy, logged.LoggedAt);

    public void Apply(IncidentCategorised categorised)
    {
        Category = categorised.Category;
        CategorisedBy = categorised.CategorisedBy;
        CategorisedAt = categorised.CategorisedAt;
    }

    public void Apply(IncidentPrioritised prioritised)
    {
        Priority = prioritised.Priority;
        PrioritisedBy = prioritised.PrioritisedBy;
        PrioritisedAt = prioritised.PrioritisedAt;
    }

    public void Apply(AgentAssignedToIncident agentAssigned)
    {
        AgentId = agentAssigned.AgentId;
        AssignedAt = agentAssigned.AssignedAt;
    }

    public void Apply(AgentRespondedToIncident agentResponded)
    {
        HasOutstandingResponseToCustomer = false;
        _responses.Add(agentResponded.Response);
    }

    public void Apply(CustomerRespondedToIncident customerResponded)
    {
        HasOutstandingResponseToCustomer = true;
        _responses.Add(customerResponded.Response);
    }

    public void Apply(IncidentResolved resolved)
    {
        Status = IncidentStatus.Resolved;
    }

    public void Apply(ResolutionAcknowledgedByCustomer acknowledged)
    {
        Status = IncidentStatus.ResolutionAcknowledgedByCustomer;
    }

    public void Apply(IncidentClosed closed)
    {
        Status = IncidentStatus.Closed;
    }
}
