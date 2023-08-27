using Raiqub.Expressions.Sessions;

namespace Helpdesk.Relational.Incidents.Categorise.v1;

public class CategoriseIncidentUseCase
{
    private readonly IDbSession _dbSession;

    public CategoriseIncidentUseCase(IDbSession dbSession)
    {
        _dbSession = dbSession;
    }

    public async Task Execute(
        Guid incidentId,
        Guid agentId,
        CategoriseIncidentRequest request,
        CancellationToken cancellationToken)
    {
        var incident = await _dbSession.Query(IncidentSpecification.HasId(incidentId)).FirstAsync(cancellationToken);
        incident.Apply(
            CategoriseScenario.Execute(incident, new CategoriseIncident(incidentId, request.Category, agentId)));

        _dbSession.Update(incident);
        await _dbSession.SaveChangesAsync(cancellationToken);
    }
}
