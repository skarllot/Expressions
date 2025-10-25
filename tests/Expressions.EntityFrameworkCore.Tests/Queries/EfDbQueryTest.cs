using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Raiqub.Common.Tests;
using Raiqub.Common.Tests.Examples;
using Raiqub.Common.Tests.Queries;
using Raiqub.Expressions.EntityFrameworkCore.Queries;
using Raiqub.Expressions.EntityFrameworkCore.Tests.Examples;
using Raiqub.Expressions.Queries;
using Xunit.Abstractions;

namespace Raiqub.Expressions.EntityFrameworkCore.Tests.Queries;

[Collection(PostgreSqlTestGroup.Name)]
public sealed class EfDbQueryTest : QueryTestBase, IAsyncLifetime
{
    private readonly PostgreSqlFixture _fixture;

    public EfDbQueryTest(PostgreSqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(
            services => services
                .AddSingleton<ILoggerFactory>(new LoggerFactory())
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
    public async Task ShouldFailWhenSourceIsNull()
    {
        var efQuery = new EfDbQuery<Blog>(NullLogger.Instance, DbQueryScope.Create<Blog>(), null!);

        await efQuery
            .Invoking(q => q.AnyAsync())
            .Should().ThrowExactlyAsync<NullReferenceException>();
        await efQuery
            .Invoking(q => q.CountAsync())
            .Should().ThrowExactlyAsync<NullReferenceException>();
        await efQuery
            .Invoking(q => q.FirstAsync())
            .Should().ThrowExactlyAsync<NullReferenceException>();
        await efQuery
            .Invoking(q => q.FirstOrDefaultAsync())
            .Should().ThrowExactlyAsync<NullReferenceException>();
        await efQuery
            .Invoking(q => q.LongCountAsync())
            .Should().ThrowExactlyAsync<NullReferenceException>();
        await efQuery
            .Invoking(q => q.ToPagedListAsync(1, 10))
            .Should().ThrowExactlyAsync<ArgumentNullException>();
        await efQuery
            .Invoking(q => q.ToListAsync())
            .Should().ThrowExactlyAsync<InvalidOperationException>();
        await efQuery
            .Invoking(q => q.SingleAsync())
            .Should().ThrowExactlyAsync<NullReferenceException>();
        await efQuery
            .Invoking(q => q.SingleOrDefaultAsync())
            .Should().ThrowExactlyAsync<NullReferenceException>();
        await efQuery
            .Invoking(async q => { await foreach (var unused in q.ToAsyncEnumerable()) { /* Testing */ } })
            .Should().ThrowExactlyAsync<InvalidOperationException>();
    }
}
