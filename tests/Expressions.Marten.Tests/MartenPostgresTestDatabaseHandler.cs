using Marten;
using Raiqub.Common.Tests;
using Raiqub.Common.Tests.Examples;
using Weasel.Core;

namespace Raiqub.Expressions.Marten.Tests;

public sealed class MartenPostgresTestDatabaseHandler : ITestDatabaseHandler
{
    private readonly PostgresTestDatabaseHandler _databaseHandler;

    public MartenPostgresTestDatabaseHandler()
    {
        _databaseHandler = new PostgresTestDatabaseHandler();
        Store = CreateDocumentStore();
    }

    public IDocumentStore Store { get; }

    public void Dispose()
    {
        Store.Dispose();
        _databaseHandler.Dispose();
    }

    private IDocumentStore CreateDocumentStore()
    {
        return DocumentStore.For(
            options =>
            {
                options.Connection(_databaseHandler.ConnectionString);
                options.AutoCreateSchemaObjects = AutoCreate.All;
                options.Schema.For<Blog>();
            });
    }
}
