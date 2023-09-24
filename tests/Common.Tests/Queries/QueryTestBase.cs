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
    [InlineData("First", 2L)]
    [InlineData("Second", 1L)]
    [InlineData("Third", 0L)]
    [InlineData("Fourth", 0L)]
    public async Task CountShouldReturnExpected(string name, long expected)
    {
        await AddBlogs(GetBlogs());
        await using var session = CreateSession();
        var query = session.Query(new GetBlogPostsAggregateQueryStrategy(name));

        long count = await query.CountAsync();

        count.Should().Be(expected);
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

    [Fact]
    public async Task ToListShouldReturnAll()
    {
        await AddBlogs(GetBlogs());
        await using var session = CreateSession();
        var query = session.Query(QueryStrategy.CreateForEntity((IQueryable<Blog> source) => source.SelectMany(b => b.Posts)));

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
            QueryStrategy.CreateForEntity(
                (IQueryable<Blog> source) => source
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

        var first = new Blog(Guid.Empty, "First");
        first.AddPost(new Post("Nice", "Keep writing", now.AddMilliseconds(1)));
        first.AddPost(new Post("The worst", "You should quit writing", now.AddMilliseconds(2)));
        yield return first;

        var second = new Blog(Guid.Empty, "Second");
        second.AddPost(new Post("Thank you", "You helped a lot", now.AddMilliseconds(1)));
        yield return second;

        yield return new Blog(Guid.Empty, "Third");
    }
}
