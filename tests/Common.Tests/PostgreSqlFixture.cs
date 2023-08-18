using DotNet.Testcontainers.Builders;
using Npgsql;
using Raiqub.Common.Tests.Commons;
using Respawn;
using Testcontainers.PostgreSql;

namespace Raiqub.Common.Tests;

public sealed class PostgreSqlFixture : IAsyncLifetime, IDisposable
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithDatabase("db")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .WithWaitStrategy(Wait.ForUnixContainer())
        .WithAutoRemove(true)
        .WithCleanUp(true)
        .Build();

    private NpgsqlConnection? _connection;
    private Respawner? _respawner;

    public string ConnectionString => _postgreSqlContainer.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        await PgIsReady.WaitAsync(ConnectionString);
    }

    public async Task DisposeAsync()
    {
        if (_connection is not null)
            await _connection.DisposeAsync();
        await _postgreSqlContainer.StopAsync();
        await _postgreSqlContainer.DisposeAsync();
    }

    public async Task SnapshotDatabaseAsync()
    {
        _connection = new NpgsqlConnection(ConnectionString);
        await _connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(
            _connection,
            new RespawnerOptions { DbAdapter = DbAdapter.Postgres });
    }

    public async Task ResetDatabaseAsync()
    {
        if (_respawner is null || _connection is null)
            return;

        await _respawner.ResetAsync(_connection);
    }

    public void Dispose()
    {
        _connection?.Dispose();
        _postgreSqlContainer.DisposeAsync().AsTask().Wait();
    }
}
