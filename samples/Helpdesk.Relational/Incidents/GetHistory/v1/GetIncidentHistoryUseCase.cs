using Raiqub.Expressions.Sessions;
using RT.Comb;

namespace Helpdesk.Relational.Incidents.GetHistory.v1;

public class GetIncidentHistoryUseCase
{
    private readonly IDbQuerySession _dbQuerySession;
    private readonly ICombProvider _combProvider;

    public GetIncidentHistoryUseCase(IDbQuerySession dbQuerySession, ICombProvider combProvider)
    {
        _dbQuerySession = dbQuerySession;
        _combProvider = combProvider;
    }

    public async Task<IReadOnlyList<IncidentHistory>> Execute(Guid incidentId, CancellationToken cancellationToken)
    {
        var incident = await _dbQuerySession
            .Query(IncidentSpecification.HasId(incidentId))
            .FirstAsync(cancellationToken);

        return CreateProjection(incident).OrderBy(h => h.Id).ToList();
    }

    private IEnumerable<IncidentHistory> CreateProjection(Incident incident)
    {
        yield return new IncidentHistory(
            _combProvider.Create(incident.Id, incident.LoggedAt.UtcDateTime),
            incident.Id,
            $"['{incident.LoggedAt}'] Logged Incident with id: '{incident.Id}' for customer '{incident.CustomerId}' and description `{incident.Description}' through {incident.Contact} by '{incident.LoggedBy}'");

        if (incident.Category is not null)
        {
            yield return new IncidentHistory(
                _combProvider.Create(incident.Id, incident.CategorisedAt!.Value.UtcDateTime),
                incident.Id,
                $"[{incident.CategorisedAt}] Categorised Incident with id: '{incident.Id}' as {incident.Category} by {incident.CategorisedBy}");
        }

        if (incident.Priority is not null)
        {
            yield return new IncidentHistory(
                _combProvider.Create(incident.Id, incident.PrioritisedAt!.Value.UtcDateTime),
                incident.Id,
                $"[{incident.PrioritisedAt}] Prioritised Incident with id: '{incident.Id}' as '{incident.Priority}' by {incident.PrioritisedBy}");
        }

        if (incident.AgentId is not null)
        {
            yield return new IncidentHistory(
                _combProvider.Create(incident.Id, incident.AssignedAt!.Value.UtcDateTime),
                incident.Id,
                $"[{incident.AssignedAt}] Assigned agent `{incident.AgentId} to incident with id: '{incident.Id}'");
        }

        foreach (var response in incident.Responses.OfType<IncidentResponse.FromCustomer>())
        {
            yield return new IncidentHistory(
                _combProvider.Create(incident.Id, response.RespondedAt.UtcDateTime),
                incident.Id,
                $"[{response.RespondedAt}] Customer '{response.CustomerId}' responded with response '{response.Content}' to Incident with id: '{incident.Id}'");
        }

        foreach (var response in incident.Responses.OfType<IncidentResponse.FromAgent>())
        {
            string responseVisibility = response.VisibleToCustomer ? "public" : "private";

            yield return new IncidentHistory(
                _combProvider.Create(incident.Id, response.RespondedAt.UtcDateTime),
                incident.Id,
                $"[{response.RespondedAt}] Agent '{response.AgentId}' responded with {responseVisibility} response '{response.Content}' to Incident with id: '{incident.Id}'");
        }
    }
}
