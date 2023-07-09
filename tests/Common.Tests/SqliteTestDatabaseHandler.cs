using Microsoft.Data.Sqlite;

namespace Raiqub.Common.Tests;

public sealed class SqliteTestDatabaseHandler : ITestDatabaseHandler
{
    public SqliteTestDatabaseHandler()
    {
        Connection = new SqliteConnection("Filename=:memory:");
        Connection.Open();
    }

    public SqliteConnection Connection { get; }

    public void Dispose() => Connection.Dispose();
}
