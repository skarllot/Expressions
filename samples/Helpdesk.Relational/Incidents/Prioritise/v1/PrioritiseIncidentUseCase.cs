using Raiqub.Expressions.Sessions;

namespace Helpdesk.Relational.Incidents.Prioritise.v1;

public class PrioritiseIncidentUseCase
{
    private readonly IDbSession _dbSession;

    public PrioritiseIncidentUseCase(IDbSession dbSession)
    {
        _dbSession = dbSession;
    }

    public async Task Execute(
        Guid incidentId,
        Guid agentId,
        PrioritiseIncidentRequest request,
        CancellationToken cancellationToken)
    {
        var incident = await _dbSession.Query(IncidentSpecification.HasId(incidentId)).FirstAsync(cancellationToken);
        incident.Apply(
            PrioritiseScenario.Execute(incident, new PrioritiseIncident(incidentId, request.Priority, agentId)));

        _dbSession.Update(incident);
        await _dbSession.SaveChangesAsync(cancellationToken);
    }
}
