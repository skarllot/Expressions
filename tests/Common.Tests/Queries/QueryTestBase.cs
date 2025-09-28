using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Raiqub.Common.Tests.Examples;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Common.Tests.Queries;

public abstract class QueryTestBase : DatabaseTestBase
{
    protected QueryTestBase(Action<IServiceCollection> registerServices)
        : base(registerServices)
    {
    }

    [Theory]
    [InlineData("First")]
    [InlineData("Second")]
    public async Task AnyShouldReturnTrue(string name)
    {
        await AddBlogs(GetBlogs());
        await using var session = CreateSession();
        var query = session.Query(new GetBlogPostsQueryStrategy(name));

        bool exists = await query.AnyAsync();

        exists.Should().BeTrue();
    }

    [Theory]
    [InlineData("Third")]
    [InlineData("Fourth")]
    [InlineData("Zero")]
    [InlineData("Other")]
    public async Task AnyShouldReturnFalse(string name)
    {
        await AddBlogs(GetBlogs());
        await using var session = CreateSession();
        var query = session.Query(new GetBlogPostsQueryStrategy(name));

        bool exists = await query.AnyAsync();

        exists.Should().BeFalse();
    }

    [Fact]
    public async Task CountAllShouldReturn3()
    {
        await AddBlogs(GetBlogs());
        await using var session = CreateSession();
        var query = session.Query(QueryStrategy.CreateNested((Blog b) => b.Posts));

        long count = await query.CountAsync();

        count.Should().Be(3);
    }

    [Theory]
    [InlineData("First", 2)]
    [InlineData("Second", 1)]
    [InlineData("Third", 0)]
    [InlineData("Fourth", 0)]
    public async Task CountShouldReturnExpected(string name, int expected)
    {
        await AddBlogs(GetBlogs());
        await using var session = CreateSession();
        var query = session.Query(new GetBlogPostsAggregateQueryStrategy(name));

        int count1 = await query.CountAsync();
        long count2 = await query.LongCountAsync();

        count1.Should().Be(expected);
        count2.Should().Be(expected);
    }

    [Theory]
    [InlineData("First", "Nice")]
    [InlineData("Second", "Thank you")]
    [InlineData("Third", null)]
    [InlineData("Fourth", null)]
    public async Task FirstOrDefaultShouldReturnExpected(string name, string? expected)
    {
        await AddBlogs(GetBlogs());
        await using var session = CreateSession();
        var query = session.Query(new GetBlogPostsQueryStrategy(name));

        Post? post = await query.FirstOrDefaultAsync();

        post?.Title.Should().Be(expected);
    }

    [Theory]
    [InlineData("First", false)]
    [InlineData("Second", false)]
    [InlineData("Third", true)]
    [InlineData("Fourth", true)]
    public async Task FirstOrDefaultWithStructShouldReturnExpected(string name, bool isNull)
    {
        await AddBlogs(GetBlogs());
        await using var session = CreateSession();
        var query = session.QueryValue(
            QueryStrategy.CreateForEntity((IQueryable<Blog> q) =>
                q.Where(b => b.Name == name).SelectMany(b => b.Posts).Select(b => b.Timestamp).OrderBy(t => t)));

        DateTimeOffset? result = await query.FirstOrDefaultAsync();

        if (isNull)
            result.Should().BeNull();
        else
            result.Should().NotBeNull();
    }

    [Fact]
    public async Task ToPagedListShouldReturnPage()
    {
        await AddBlogs(GetBlogs());
        await using var session = CreateSession();
        var query = session.Query<Blog>();

        var pagedResult1 = await query.ToPagedListAsync(1, 10);
        var pagedResult2 = await query.ToPagedListAsync(2, 2);
        var pagedResult3 = await query.ToPagedListAsync(3, 2);

        pagedResult1.TotalCount.Should().Be(3);
        pagedResult1.Should().HaveCount(3);
        pagedResult1.PageNumber.Should().Be(1);
        pagedResult1.PageSize.Should().Be(10);
        pagedResult1.IsFirstPage.Should().BeTrue();
        pagedResult1.IsLastPage.Should().BeTrue();
        pagedResult1.HasNextPage.Should().BeFalse();
        pagedResult1.HasPreviousPage.Should().BeFalse();
        pagedResult1.PageCount.Should().Be(1);
        pagedResult1.FirstItemOnPage.Should().Be(1);
        pagedResult1.LastItemOnPage.Should().Be(3);
        pagedResult1.Count.Should().Be(3);
        pagedResult1[0].Name.Should().Be("First");
        pagedResult1[1].Name.Should().Be("Second");
        pagedResult1[2].Name.Should().Be("Third");

        pagedResult2.TotalCount.Should().Be(3);
        pagedResult2.Should().HaveCount(1);
        pagedResult2.PageNumber.Should().Be(2);
        pagedResult2.PageSize.Should().Be(2);
        pagedResult2.IsFirstPage.Should().BeFalse();
        pagedResult2.IsLastPage.Should().BeTrue();
        pagedResult2.HasNextPage.Should().BeFalse();
        pagedResult2.HasPreviousPage.Should().BeTrue();
        pagedResult2.PageCount.Should().Be(2);
        pagedResult2.FirstItemOnPage.Should().Be(3);
        pagedResult2.LastItemOnPage.Should().Be(3);
        pagedResult2.Count.Should().Be(1);
        pagedResult2[0].Name.Should().Be("Third");

        pagedResult3.TotalCount.Should().Be(0);
        pagedResult3.Should().BeEmpty();
        pagedResult3.PageNumber.Should().Be(3);
        pagedResult3.PageSize.Should().Be(2);
        pagedResult3.IsFirstPage.Should().BeFalse();
        pagedResult3.IsLastPage.Should().BeFalse();
        pagedResult3.HasNextPage.Should().BeFalse();
        pagedResult3.HasPreviousPage.Should().BeFalse();
        pagedResult3.PageCount.Should().Be(0);
        pagedResult3.FirstItemOnPage.Should().Be(0);
        pagedResult3.LastItemOnPage.Should().Be(0);
        pagedResult3.Count.Should().Be(0);
    }

    [Fact]
    public async Task ToListShouldReturnAll()
    {
        await AddBlogs(GetBlogs());
        await using var session = CreateSession();
        var query = session.Query(
            QueryStrategy.CreateForEntity((IQueryable<Blog> source) => source.SelectMany(b => b.Posts)));

        var posts = await query.ToListAsync();

        posts.Should().HaveCount(3);
        posts.Select(p => p.Title).Should().BeEquivalentTo("Nice", "The worst", "Thank you");
    }

    [Fact]
    public async Task ToListShouldReturnExpected()
    {
        await AddBlogs(GetBlogs());
        await using var session = CreateSession();
        var query = session.Query(
            QueryStrategy.CreateForEntity((IQueryable<Blog> source) => source
                .SelectMany(b => b.Posts)
                .Where(p => p.Content.StartsWith("You"))));

        var posts = await query.ToListAsync();

        posts.Should().HaveCount(2);
        posts.Select(p => p.Title).Should().BeEquivalentTo("The worst", "Thank you");
    }

    [Theory]
    [InlineData("Second", "Thank you")]
    [InlineData("Third", null)]
    [InlineData("Fourth", null)]
    public async Task SingleOrDefaultShouldReturnExpected(string name, string? expected)
    {
        await AddBlogs(GetBlogs());
        await using var session = CreateSession();
        var query = session.Query(new GetBlogPostsQueryStrategy(name));

        Post? post = await query.SingleOrDefaultAsync();

        post?.Title.Should().Be(expected);
    }

    [Theory]
    [InlineData("Second", false)]
    [InlineData("Third", true)]
    [InlineData("Fourth", true)]
    public async Task SingleOrDefaultWithStructShouldReturnExpected(string name, bool isNull)
    {
        await AddBlogs(GetBlogs());
        await using var session = CreateSession();
        var query = session.QueryValue(
            QueryStrategy.CreateForEntity((IQueryable<Blog> q) =>
                q.Where(b => b.Name == name).SelectMany(b => b.Posts).Select(b => b.Timestamp).OrderBy(t => t)));

        DateTimeOffset? result = await query.SingleOrDefaultAsync();

        if (isNull)
            result.Should().BeNull();
        else
            result.Should().NotBeNull();
    }

    [Theory]
    [InlineData("First")]
    public async Task SingleOrDefaultShouldFail(string name)
    {
        await AddBlogs(GetBlogs());
        await using var session = CreateSession();
        var query = session.Query(new GetBlogPostsQueryStrategy(name));

        await query
            .Invoking(q => q.SingleOrDefaultAsync())
            .Should().ThrowExactlyAsync<InvalidOperationException>();
    }

    [Theory]
    [InlineData("First")]
    public async Task SingleOrDefaultWithStructShouldFail(string name)
    {
        await AddBlogs(GetBlogs());
        await using var session = CreateSession();
        var query = session.QueryValue(
            QueryStrategy.CreateForEntity((IQueryable<Blog> q) =>
                q.Where(b => b.Name == name).SelectMany(b => b.Posts).Select(b => b.Timestamp).OrderBy(t => t)));

        await query
            .Invoking(q => q.SingleOrDefaultAsync())
            .Should().ThrowExactlyAsync<InvalidOperationException>();
    }

    private IDbQuerySession CreateSession() => ServiceProvider.GetRequiredService<IDbQuerySession>();

    private async Task AddBlogs(IEnumerable<Blog> blogs)
    {
        IDbSessionFactory dbSessionFactory = ServiceProvider.GetRequiredService<IDbSessionFactory>();
        await using IDbSession dbSession = dbSessionFactory.Create();
        await dbSession.AddRangeAsync(blogs);
        await dbSession.SaveChangesAsync();
    }

    private static IEnumerable<Blog> GetBlogs()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;

        var first = new Blog(new Guid("018a7015-fd5b-48a2-9ffa-07ef1ce7486d"), "First");
        first.AddPost(new Post("Nice", "Keep writing", now.AddMilliseconds(1)));
        first.AddPost(new Post("The worst", "You should quit writing", now.AddMilliseconds(2)));
        yield return first;

        var second = new Blog(new Guid("018a7016-05a4-48c3-8545-63549cd3aeed"), "Second");
        second.AddPost(new Post("Thank you", "You helped a lot", now.AddMilliseconds(1)));
        yield return second;

        yield return new Blog(new Guid("018a7018-8fee-4acf-968b-5c89f5599f23"), "Third");
    }
}
