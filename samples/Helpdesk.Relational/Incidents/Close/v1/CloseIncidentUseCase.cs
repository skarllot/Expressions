using Raiqub.Expressions.Sessions;

namespace Helpdesk.Relational.Incidents.Close.v1;

public class CloseIncidentUseCase
{
    private readonly IDbSession _dbSession;

    public CloseIncidentUseCase(IDbSession dbSession)
    {
        _dbSession = dbSession;
    }

    public async Task Execute(Guid incidentId, Guid agentId, CancellationToken cancellationToken)
    {
        var incident = await _dbSession.Query(IncidentSpecification.HasId(incidentId)).FirstAsync(cancellationToken);
        incident.Apply(CloseScenario.Execute(incident, new CloseIncident(incidentId, agentId)));

        _dbSession.Update(incident);
        await _dbSession.SaveChangesAsync(cancellationToken);
    }
}
