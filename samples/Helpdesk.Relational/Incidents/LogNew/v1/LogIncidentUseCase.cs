using Raiqub.Expressions.Sessions;
using RT.Comb;

namespace Helpdesk.Relational.Incidents.LogNew.v1;

public class LogIncidentUseCase
{
    private readonly ICombProvider _combProvider;
    private readonly IDbSession _dbSession;

    public LogIncidentUseCase(ICombProvider combProvider, IDbSession dbSession)
    {
        _combProvider = combProvider;
        _dbSession = dbSession;
    }

    public async Task<Incident> Execute(Guid customerId, LogIncidentRequest request, CancellationToken cancellationToken)
    {
        var incident = Incident.Create(
            LogNewScenario.Execute(
                new LogIncident(
                    _combProvider.Create(),
                    customerId,
                    request.Contact,
                    request.Description,
                    customerId)));

        await _dbSession.AddAsync(incident, cancellationToken);
        await _dbSession.SaveChangesAsync(cancellationToken);

        return incident;
    }
}
