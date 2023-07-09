using MysticMind.PostgresEmbed;
using Raiqub.Common.Tests.Commons;

namespace Raiqub.Common.Tests;

public sealed class PostgresTestDatabaseHandler : ITestDatabaseHandler
{
    private readonly PgServer _pgServer = new(pgVersion: "10.7.1");

    public PostgresTestDatabaseHandler()
    {
        _pgServer.Start();
        PgIsReady.Wait(ConnectionString);
    }

    public string ConnectionString =>
        $"Server=localhost;Port={_pgServer.PgPort};User Id={_pgServer.PgUser};Password=test;Database={_pgServer.PgDbName};Pooling=false";

    public void Dispose()
    {
        _pgServer.Dispose();
    }
}
