using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Raiqub.Common.Tests;
using Raiqub.Common.Tests.Examples;
using Raiqub.Common.Tests.Sessions;
using Raiqub.Expressions.EntityFrameworkCore.Tests.Examples;
using Xunit.Abstractions;

namespace Raiqub.Expressions.EntityFrameworkCore.Tests.Sessions;

[Collection(PostgreSqlTestGroup.Name)]
public sealed class EfSessionFactoryTest : SessionFactoryTestBase, IAsyncLifetime
{
    private readonly PostgreSqlFixture _fixture;

    public EfSessionFactoryTest(PostgreSqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(
            services => services
                .AddSingleton<ILoggerFactory>(new NullLoggerFactory())
                .AddPostgreSqlDbContext<BloggingContext>(testOutputHelper, fixture.ConnectionString)
                .AddEntityFrameworkExpressions()
                .AddSingleContext<BloggingContext>())
    {
        _fixture = fixture;
        ServiceProvider.GetRequiredService<BloggingContext>().Database.EnsureCreated();
    }

    public Task InitializeAsync() => _fixture.SnapshotDatabaseAsync();

    public Task DisposeAsync() => _fixture.ResetDatabaseAsync();

    [Fact]
    public async Task AddAndSaveOnUncommittedTransactionShouldNotCommitChanges()
    {
        var sessionFactory = CreateSessionFactory();

        await using (var session = sessionFactory.Create())
        await using (await session.BeginTransactionAsync())
        {
            await session.AddAsync(GetBlogs().First());
            await session.SaveChangesAsync();
        }

        int count;
        await using (var session = sessionFactory.Create())
        {
            count = await session.Query<Blog>().CountAsync();
        }

        count.Should().Be(0);
    }

    [Fact]
    public async Task AddAndSaveOnCommittedTransactionShouldCommitChanges()
    {
        var sessionFactory = CreateSessionFactory();

        await using (var session = sessionFactory.Create())
        await using (var transaction = await session.BeginTransactionAsync())
        {
            await session.AddAsync(GetBlogs().First());
            await session.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        int count;
        await using (var session = sessionFactory.Create())
        {
            count = await session.Query<Blog>().CountAsync();
        }

        count.Should().Be(1);
    }
}
