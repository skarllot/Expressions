using AwesomeAssertions;
using Helpdesk.Relational.Incidents;
using Helpdesk.Relational.Incidents.LogNew;
using Helpdesk.Relational.Incidents.LogNew.v1;
using NSubstitute;
using NSubstitute.Extensions;
using Raiqub.Expressions.Sessions;
using RT.Comb;

namespace Helpdesk.Relational.Tests.Incidents.LogNew.v1;

public class LogIncidentUseCaseTest
{
    private readonly ICombProvider _combProvider;
    private readonly IDbSession _dbSession;
    private readonly LogIncidentUseCase _useCase;

    public LogIncidentUseCaseTest()
    {
        _combProvider = Substitute.For<ICombProvider>();
        _dbSession = Substitute.For<IDbSession>();
        _useCase = new LogIncidentUseCase(_combProvider, _dbSession);
    }

    [Fact]
    public async Task ShouldCreateAndSaveNewIncident()
    {
        // Arrange
        LogIncident logNewCommand = IncidentFixture.LogNew();

        _combProvider.Configure().Create().Returns(logNewCommand.IncidentId);

        // Act
        Incident incident = await _useCase.Execute(
            logNewCommand.CustomerId,
            new LogIncidentRequest(logNewCommand.Contact, logNewCommand.Description),
            CancellationToken.None);

        // Assert
        await _dbSession.Received(1).AddAsync(incident);
        await _dbSession.Received(1).SaveChangesAsync();
        _dbSession.ReceivedCalls().Should().HaveCount(2);

        incident.Id.Should().Be(logNewCommand.IncidentId);
        incident.Status.Should().Be(IncidentStatus.Pending);
        incident.Contact.ContactChannel.Should().Be(ContactChannel.Phone);
    }
}
