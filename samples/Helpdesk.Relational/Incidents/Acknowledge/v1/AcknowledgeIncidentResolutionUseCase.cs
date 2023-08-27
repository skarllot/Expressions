using Raiqub.Expressions.Sessions;

namespace Helpdesk.Relational.Incidents.Acknowledge.v1;

public class AcknowledgeIncidentResolutionUseCase
{
    private readonly IDbSession _dbSession;

    public AcknowledgeIncidentResolutionUseCase(IDbSession dbSession)
    {
        _dbSession = dbSession;
    }

    public async Task Execute(Guid incidentId, Guid customerId, CancellationToken cancellationToken)
    {
        var incident = await _dbSession.Query(IncidentSpecification.HasId(incidentId)).FirstAsync(cancellationToken);
        incident.Apply(AcknowledgeScenario.Execute(incident, new AcknowledgeResolution(incidentId, customerId)));

        _dbSession.Update(incident);
        await _dbSession.SaveChangesAsync(cancellationToken);
    }
}
