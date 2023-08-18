using Marten;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Raiqub.Common.Tests;
using Raiqub.Common.Tests.Examples;
using Raiqub.Common.Tests.Sessions;

namespace Raiqub.Expressions.Marten.Tests.Sessions;

[Collection(PostgreSqlTestGroup.Name)]
public sealed class MartenSessionFactoryTest : SessionFactoryTestBase, IAsyncLifetime
{
    public MartenSessionFactoryTest(PostgreSqlFixture fixture)
        : base(
            services => services
                .AddSingleton<ILoggerFactory>(new NullLoggerFactory())
                .AddTestMarten<Blog>(fixture.ConnectionString)
                .AddMartenExpressions()
                .AddSingleContext())
    {
    }

    public async Task InitializeAsync()
    {
        var store = ServiceProvider.GetRequiredService<IDocumentStore>();
        await store.Advanced.ResetAllData();
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
