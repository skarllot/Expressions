using System.Data;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Npgsql;
using Testcontainers.PostgreSql;

namespace Raiqub.Common.Tests.Commons;

public sealed class PgIsReady : IWaitUntil
{
    public Task<bool> UntilAsync(IContainer container)
    {
        return UntilAsync((PostgreSqlContainer)container);
    }

    private static async Task<bool> UntilAsync(PostgreSqlContainer container)
    {
        using var cts = new CancellationTokenSource(3000);

        try
        {
            await using var connection = new NpgsqlConnection(container.GetConnectionString());
            await connection.OpenAsync(cts.Token);
            return connection.State == ConnectionState.Open;
        }
        catch (NpgsqlException)
        {
            return false;
        }
    }
}
