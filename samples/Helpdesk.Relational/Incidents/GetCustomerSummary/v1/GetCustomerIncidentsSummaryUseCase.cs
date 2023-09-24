using Raiqub.Expressions.Sessions;

namespace Helpdesk.Relational.Incidents.GetCustomerSummary.v1;

public class GetCustomerIncidentsSummaryUseCase
{
    private readonly IDbQuerySession _dbQuerySession;

    public GetCustomerIncidentsSummaryUseCase(IDbQuerySession dbQuerySession)
    {
        _dbQuerySession = dbQuerySession;
    }

    public async Task<CustomerIncidentsSummary> Execute(Guid customerId, CancellationToken cancellationToken)
    {
        return await _dbQuerySession
            .Query(new GetCustomerIncidentsSummaryQueryStrategy(customerId))
            .ToAsyncEnumerable(cancellationToken)
            .AggregateAsync(
                new CustomerIncidentsSummary(customerId),
                (seed, acc) => acc.Key switch
                {
                    IncidentStatus.Pending => seed with { Pending = acc.Count },
                    IncidentStatus.Resolved => seed with { Resolved = acc.Count },
                    IncidentStatus.ResolutionAcknowledgedByCustomer => seed with { Acknowledged = acc.Count },
                    IncidentStatus.Closed => seed with { Closed = acc.Count },
                    _ => throw new InvalidOperationException()
                },
                cancellationToken);
    }
}
