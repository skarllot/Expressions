using Raiqub.Expressions.Sessions;

namespace Helpdesk.Relational.Incidents.RecordCustomerResponse.v1;

public class RecordCustomerResponseToIncidentUseCase
{
    private readonly IDbSession _dbSession;

    public RecordCustomerResponseToIncidentUseCase(IDbSession dbSession)
    {
        _dbSession = dbSession;
    }

    public async Task Execute(
        Guid incidentId,
        Guid customerId,
        RecordCustomerResponseToIncidentRequest request,
        CancellationToken cancellationToken)
    {
        var incident = await _dbSession.Query(IncidentSpecification.HasId(incidentId)).FirstAsync(cancellationToken);
        incident.Apply(
            RecordCustomerResponseScenario.Execute(
                incident,
                new RecordCustomerResponseToIncident(
                    incidentId,
                    new IncidentResponse.FromCustomer(customerId, request.Content, DateTimeOffset.UtcNow))));

        _dbSession.Update(incident);
        await _dbSession.SaveChangesAsync(cancellationToken);
    }
}
