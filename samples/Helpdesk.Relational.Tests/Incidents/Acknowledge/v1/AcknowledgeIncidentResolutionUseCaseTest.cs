using FluentAssertions;
using Helpdesk.Relational.Incidents;
using Helpdesk.Relational.Incidents.Acknowledge;
using Helpdesk.Relational.Incidents.Acknowledge.v1;
using Helpdesk.Relational.Tests.Common;
using NSubstitute;
using NSubstitute.Extensions;
using Raiqub.Expressions;
using Raiqub.Expressions.Sessions;

namespace Helpdesk.Relational.Tests.Incidents.Acknowledge.v1;

public class AcknowledgeIncidentResolutionUseCaseTest
{
    private readonly IDbSession _dbSession;
    private readonly AcknowledgeIncidentResolutionUseCase _useCase;

    public AcknowledgeIncidentResolutionUseCaseTest()
    {
        _dbSession = Substitute.For<IDbSession>();
        _useCase = new AcknowledgeIncidentResolutionUseCase(_dbSession);
    }

    [Fact]
    public async Task ShouldAcknowledgeResolvedIncident()
    {
        // Arrange
        var incident = IncidentFixture.Resolved();

        _dbSession.Configure().Query(Arg.Any<Specification<Incident>>()).FirstAsync().Returns(incident);

        // Act
        await _useCase.Execute(incident.Id, incident.CustomerId, CancellationToken.None);

        // Assert
        await _dbSession
            .Received(1).Query(Arg.Any<Specification<Incident>>())
            .Received(1).FirstAsync();

        _dbSession.Received(1).Update(incident);
        await _dbSession.Received(1).SaveChangesAsync();
        _dbSession.ReceivedCalls().Should().HaveCount(3);

        incident.Status.Should().Be(IncidentStatus.ResolutionAcknowledgedByCustomer);
    }

    [Theory]
    [MemberData(nameof(Unresolved))]
    public async Task ShouldFailAcknowledgeForUnresolvedIncident(Incident incident)
    {
        // Arrange
        _dbSession.Configure().Query(Arg.Any<Specification<Incident>>()).FirstAsync().Returns(incident);

        // Act
        Func<Task> useCaseExecute = () => _useCase.Execute(incident.Id, incident.CustomerId, CancellationToken.None);

        // Arrange
        await useCaseExecute.Should().ThrowExactlyAsync<IncidentIsNotResolvedException>();

        await _dbSession.DidNotReceive().SaveChangesAsync();

        incident.Status.Should().NotBe(IncidentStatus.ResolutionAcknowledgedByCustomer);
    }

    public static TheoryData<Incident> Unresolved => TheoryBuilder.Build(
        IncidentFixture.Logged(),
        IncidentFixture.AgentAssigned());
}
