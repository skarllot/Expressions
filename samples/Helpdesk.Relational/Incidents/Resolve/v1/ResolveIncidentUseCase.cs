using Raiqub.Expressions.Sessions;

namespace Helpdesk.Relational.Incidents.Resolve.v1;

public class ResolveIncidentUseCase
{
    private readonly IDbSession _dbSession;

    public ResolveIncidentUseCase(IDbSession dbSession)
    {
        _dbSession = dbSession;
    }

    public async Task Execute(
        Guid incidentId,
        Guid agentId,
        ResolveIncidentRequest request,
        CancellationToken cancellationToken)
    {
        var incident = await _dbSession.Query(IncidentSpecification.HasId(incidentId)).FirstAsync(cancellationToken);
        incident.Apply(ResolveScenario.Execute(incident, new ResolveIncident(incidentId, request.Resolution, agentId)));

        _dbSession.Update(incident);
        await _dbSession.SaveChangesAsync(cancellationToken);
    }
}
