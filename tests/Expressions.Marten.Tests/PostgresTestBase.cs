using DotNet.Testcontainers.Builders;
using Marten;
using Raiqub.Common.Tests.Examples;
using Raiqub.Expressions.Marten.Tests.Commons;
using Testcontainers.PostgreSql;
using Weasel.Core;

namespace Raiqub.Expressions.Marten.Tests;

public abstract class PostgresTestBase : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container =
        new PostgreSqlBuilder().WithWaitStrategy(Wait.ForUnixContainer()).Build();

    private IDocumentStore? _store;

    private string ConnectionString => _container.GetConnectionString();

    protected IDocumentStore Store =>
        _store ?? throw new InvalidOperationException("Document store could not be initialized");

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        await PgIsReady.Wait(ConnectionString);
        _store = CreateDocumentStore();
        await InitializeData();
    }

    public async Task DisposeAsync()
    {
        _store?.Dispose();
        await _container.DisposeAsync();
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
