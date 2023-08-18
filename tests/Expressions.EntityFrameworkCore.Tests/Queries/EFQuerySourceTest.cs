using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Raiqub.Common.Tests;
using Raiqub.Common.Tests.Examples;
using Raiqub.Expressions.EntityFrameworkCore.Queries;
using Raiqub.Expressions.EntityFrameworkCore.Tests.Examples;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Expressions.EntityFrameworkCore.Tests.Queries;

[Collection(PostgreSqlTestGroup.Name)]
public class EFQuerySourceTest : DatabaseTestBase, IAsyncLifetime
{
    private readonly PostgreSqlFixture _fixture;

    public EFQuerySourceTest(PostgreSqlFixture fixture)
        : base(
            services => services
                .AddPostgreSqlDbContext<BloggingContext>(fixture.ConnectionString))
    {
        _fixture = fixture;
        ServiceProvider.GetRequiredService<BloggingContext>().Database.EnsureCreated();
    }

    private BloggingContext DbContext => ServiceProvider.GetRequiredService<BloggingContext>();

    public Task InitializeAsync() => _fixture.SnapshotDatabaseAsync();

    public Task DisposeAsync() => _fixture.ResetDatabaseAsync();

    [Fact]
    public void GetBlogSetShouldReturnExpected()
    {
        var querySource = new EFQuerySource(DbContext, ChangeTracking.Default);

        var blogs = querySource.GetSet<Blog>();

        blogs.Should().BeEmpty();
    }

    [Fact]
    public void GetPostSetShouldReturnExpected()
    {
        var querySource = new EFQuerySource(DbContext, ChangeTracking.Disable);

        var blogs = querySource.GetSet<Post>();

        blogs.Should().BeEmpty();
    }

    [Fact]
    public void GetVideoPostSetShouldReturnExpected()
    {
        var querySource = new EFQuerySource(DbContext, ChangeTracking.Enable);

        var blogs = querySource.GetSet<VideoPost>();

        blogs.Should().BeEmpty();
    }
}
