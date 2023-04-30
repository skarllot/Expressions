using Npgsql;

namespace Raiqub.Expressions.Marten.Tests.Commons;

public static class PgIsReady
{
    public static async Task Wait(string connectionString)
    {
        using var cts = new CancellationTokenSource(3000);

        while (cts.IsCancellationRequested is false)
        {
            try
            {
                await using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync(cts.Token);
                break;
            }
            catch (NpgsqlException)
            {
                await Task.Delay(250, cts.Token);
            }
        }
    }
}
