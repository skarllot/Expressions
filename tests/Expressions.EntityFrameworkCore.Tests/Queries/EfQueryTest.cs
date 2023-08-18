using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Raiqub.Common.Tests;
using Raiqub.Common.Tests.Examples;
using Raiqub.Common.Tests.Queries;
using Raiqub.Expressions.EntityFrameworkCore.Queries;
using Raiqub.Expressions.EntityFrameworkCore.Tests.Examples;

namespace Raiqub.Expressions.EntityFrameworkCore.Tests.Queries;

[Collection(PostgreSqlTestGroup.Name)]
public sealed class EfQueryTest : QueryTestBase, IAsyncLifetime
{
    private readonly PostgreSqlFixture _fixture;

    public EfQueryTest(PostgreSqlFixture fixture)
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

    [Fact]
    public async Task ShouldFailWhenSourceIsNull()
    {
        var efQuery = new EfQuery<Blog>(NullLogger.Instance, null!);

        await efQuery
            .Invoking(q => q.AnyAsync())
            .Should().ThrowExactlyAsync<ArgumentNullException>();
        await efQuery
            .Invoking(q => q.CountAsync())
            .Should().ThrowExactlyAsync<ArgumentNullException>();
        await efQuery
            .Invoking(q => q.FirstOrDefaultAsync())
            .Should().ThrowExactlyAsync<ArgumentNullException>();
        await efQuery
            .Invoking(q => q.ToListAsync())
            .Should().ThrowExactlyAsync<ArgumentNullException>();
        await efQuery
            .Invoking(q => q.SingleOrDefaultAsync())
            .Should().ThrowExactlyAsync<ArgumentNullException>();
    }
}
