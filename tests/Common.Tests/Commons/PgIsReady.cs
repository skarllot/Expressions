using Npgsql;

namespace Raiqub.Common.Tests.Commons;

public static class PgIsReady
{
    public static void Wait(string connectionString)
    {
        using var timeoutEvent = new ManualResetEventSlim(false);

        using var timer = new Timer(
            static m => ((ManualResetEventSlim)m!).Set(),
            timeoutEvent,
            3000,
            Timeout.Infinite);

        while (true)
        {
            try
            {
                using var connection = new NpgsqlConnection(connectionString);
                connection.Open();
                break;
            }
            catch (NpgsqlException)
            {
                if (timeoutEvent.Wait(250))
                {
                    throw;
                }
            }
        }
    }

    public static async Task WaitAsync(string connectionString)
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
