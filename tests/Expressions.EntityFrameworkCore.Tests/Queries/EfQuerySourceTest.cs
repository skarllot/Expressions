using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Raiqub.Common.Tests;
using Raiqub.Common.Tests.Examples;
using Raiqub.Expressions.EntityFrameworkCore.Options;
using Raiqub.Expressions.EntityFrameworkCore.Queries;
using Raiqub.Expressions.EntityFrameworkCore.Tests.Examples;
using Raiqub.Expressions.Sessions;
using Xunit.Abstractions;

namespace Raiqub.Expressions.EntityFrameworkCore.Tests.Queries;

[Collection(PostgreSqlTestGroup.Name)]
public class EfQuerySourceTest : DatabaseTestBase, IAsyncLifetime
{
    private readonly PostgreSqlFixture _fixture;

    public EfQuerySourceTest(PostgreSqlFixture fixture, ITestOutputHelper testOutputHelper)
        : base(
            services => services
                .AddPostgreSqlDbContext<BloggingContext>(testOutputHelper, fixture.ConnectionString))
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
        var querySource = new EfQuerySource(
            DbContext,
            SqlProviderSelector.Empty,
            EntityOptionsSelector.Empty,
            ChangeTracking.Default);

        var blogs = querySource.GetSet<Blog>();

        blogs.Should().BeEmpty();
    }

    [Fact]
    public void GetPostSetShouldReturnExpected()
    {
        var querySource = new EfQuerySource(
            DbContext,
            SqlProviderSelector.Empty,
            EntityOptionsSelector.Empty,
            ChangeTracking.Disable);

        var blogs = querySource.GetSet<Post>();

        blogs.Should().BeEmpty();
    }

    [Fact]
    public void GetVideoPostSetShouldReturnExpected()
    {
        var querySource = new EfQuerySource(
            DbContext,
            SqlProviderSelector.Empty,
            EntityOptionsSelector.Empty,
            ChangeTracking.Enable);

        var blogs = querySource.GetSet<VideoPost>();

        blogs.Should().BeEmpty();
    }

    [Fact]
    public void GetBlogSetUsingCustomSqlShouldReturnExpected()
    {
        var querySource = new EfQuerySource(
            DbContext,
            new SqlProviderSelector(new[] { new BlogSqlProvider() }),
            EntityOptionsSelector.Empty,
            ChangeTracking.Default);

        DbContext.AddRange(
            new Blog(new Guid("018a7015-fd5b-48a2-9ffa-07ef1ce7486d"), "John"),
            new Blog(new Guid("018a7016-05a4-48c3-8545-63549cd3aeed"), "Jane"),
            new Blog(new Guid("018a7018-8fee-4acf-968b-5c89f5599f23"), "Alice"));

        DbContext.SaveChanges();
        DbContext.ChangeTracker.Clear();

        var blogsFromSql = querySource.GetSet<Blog>().ToList();

        blogsFromSql.Select(b => b.Name).Should().Equal("<John>", "<Jane>", "<Alice>");
    }

    [Fact]
    public void GetBlogSetUsingSplitQueriesShouldReturnExpected()
    {
        var querySource = new EfQuerySource(
            DbContext,
            SqlProviderSelector.Empty,
            new EntityOptionsSelector(
                new[] { new EntityOptionsConfiguration(typeof(Blog), options => options.UseSplitQuery = true) }),
            ChangeTracking.Default);

        DbContext.AddRange(
            new Blog(new Guid("018a7015-fd5b-48a2-9ffa-07ef1ce7486d"), "John"),
            new Blog(new Guid("018a7016-05a4-48c3-8545-63549cd3aeed"), "Jane"),
            new Blog(new Guid("018a7018-8fee-4acf-968b-5c89f5599f23"), "Alice"));

        DbContext.SaveChanges();
        DbContext.ChangeTracker.Clear();

        var blogsFromSql = querySource.GetSet<Blog>().Where(x => x.Name.Contains("n")).ToList();

        blogsFromSql.Select(b => b.Name).Should().Equal("John", "Jane");
    }

    private class BlogSqlProvider : ISqlProvider<Blog>
    {
        public SqlString GetQuerySql() =>
            SqlString.FromSqlInterpolated($"SELECT \"Id\", '<' || \"Name\" || '>' as \"Name\" FROM \"Blog\"");
    }
}
