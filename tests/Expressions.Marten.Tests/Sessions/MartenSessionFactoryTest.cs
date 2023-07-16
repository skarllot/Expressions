using Microsoft.Extensions.Logging.Abstractions;
using Raiqub.Common.Tests.Sessions;
using Raiqub.Expressions.Marten.Sessions;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Expressions.Marten.Tests.Sessions;

public sealed class MartenSessionFactoryTest : SessionFactoryTestBase, IDisposable
{
    private readonly MartenPostgresTestDatabaseHandler _databaseHandler;

    public MartenSessionFactoryTest()
    {
        _databaseHandler = new MartenPostgresTestDatabaseHandler();
    }

    public void Dispose() => _databaseHandler.Dispose();

    protected override ISessionFactory CreateSessionFactory()
    {
        return new MartenSessionFactory(new NullLoggerFactory(), _databaseHandler.Store);
    }
}
