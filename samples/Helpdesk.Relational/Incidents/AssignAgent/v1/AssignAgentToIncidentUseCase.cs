using Raiqub.Expressions.Sessions;

namespace Helpdesk.Relational.Incidents.AssignAgent.v1;

public class AssignAgentToIncidentUseCase
{
    private readonly IDbSession _dbSession;

    public AssignAgentToIncidentUseCase(IDbSession dbSession)
    {
        _dbSession = dbSession;
    }

    public async Task Execute(Guid incidentId, Guid agentId, CancellationToken cancellationToken)
    {
        var incident = await _dbSession.Query(IncidentSpecification.HasId(incidentId)).FirstAsync(cancellationToken);
        incident.Apply(AssignAgentScenario.Execute(incident, new AssignAgentToIncident(incidentId, agentId)));

        _dbSession.Update(incident);
        await _dbSession.SaveChangesAsync(cancellationToken);
    }
}
