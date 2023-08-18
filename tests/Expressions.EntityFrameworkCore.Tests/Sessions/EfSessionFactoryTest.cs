using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Raiqub.Common.Tests;
using Raiqub.Common.Tests.Sessions;
using Raiqub.Expressions.EntityFrameworkCore.Tests.Examples;

namespace Raiqub.Expressions.EntityFrameworkCore.Tests.Sessions;

[Collection(PostgreSqlTestGroup.Name)]
public sealed class EfSessionFactoryTest : SessionFactoryTestBase, IAsyncLifetime
{
    private readonly PostgreSqlFixture _fixture;

    public EfSessionFactoryTest(PostgreSqlFixture fixture)
        : base(
            services => services
                .AddSingleton<ILoggerFactory>(new NullLoggerFactory())
                .AddPostgreSqlDbContext<BloggingContext>(fixture.ConnectionString)
                .AddEntityFrameworkExpressions()
                .AddSingleContext<BloggingContext>())
    {
        _fixture = fixture;
        ServiceProvider.GetRequiredService<BloggingContext>().Database.EnsureCreated();
    }

    public Task InitializeAsync() => _fixture.SnapshotDatabaseAsync();

    public Task DisposeAsync() => _fixture.ResetDatabaseAsync();
}
