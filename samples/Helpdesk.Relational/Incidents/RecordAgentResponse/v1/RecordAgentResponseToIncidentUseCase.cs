using Raiqub.Expressions.Sessions;

namespace Helpdesk.Relational.Incidents.RecordAgentResponse.v1;

public class RecordAgentResponseToIncidentUseCase
{
    private readonly IDbSession _dbSession;

    public RecordAgentResponseToIncidentUseCase(IDbSession dbSession)
    {
        _dbSession = dbSession;
    }

    public async Task Execute(
        Guid incidentId,
        Guid agentId,
        RecordAgentResponseToIncidentRequest request,
        CancellationToken cancellationToken)
    {
        var incident = await _dbSession.Query(IncidentSpecification.HasId(incidentId)).FirstAsync(cancellationToken);
        incident.Apply(
            RecordAgentResponseScenario.Execute(
                incident,
                new RecordAgentResponseToIncident(
                    incidentId,
                    new IncidentResponse.FromAgent(
                        agentId,
                        request.Content,
                        request.VisibleToCustomer,
                        DateTimeOffset.UtcNow))));

        _dbSession.Update(incident);
        await _dbSession.SaveChangesAsync(cancellationToken);
    }
}
