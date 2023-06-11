using Marten;
using MysticMind.PostgresEmbed;
using Raiqub.Common.Tests.Examples;
using Raiqub.Expressions.Marten.Tests.Commons;
using Weasel.Core;

namespace Raiqub.Expressions.Marten.Tests;

public abstract class PostgresTestBase : IAsyncLifetime, IDisposable
{
    private readonly PgServer _pgServer = new(pgVersion: "10.7.1");

    private IDocumentStore? _store;

    private string ConnectionString => $"Server=localhost;Port={_pgServer.PgPort};User Id={_pgServer.PgUser};Password=test;Database={_pgServer.PgDbName};Pooling=false";

    protected IDocumentStore Store =>
        _store ?? throw new InvalidOperationException("Document store could not be initialized");

    public async Task InitializeAsync()
    {
        await _pgServer.StartAsync();
        await PgIsReady.Wait(ConnectionString);
        _store = CreateDocumentStore();
        await InitializeData();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _store?.Dispose();
            _pgServer.Dispose();
        }
    }

    protected abstract void InitializeData(IDocumentSession session);

    private async Task InitializeData()
    {
        await using IDocumentSession session = Store.LightweightSession();
        InitializeData(session);
        await session.SaveChangesAsync();
    }

    private IDocumentStore CreateDocumentStore()
    {
        return DocumentStore.For(
            options =>
            {
                options.Connection(ConnectionString);
                options.AutoCreateSchemaObjects = AutoCreate.All;
                options.Schema.For<Blog>();
            });
    }
}
