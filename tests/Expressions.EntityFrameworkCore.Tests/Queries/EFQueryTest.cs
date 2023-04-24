using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Raiqub.Expressions.EntityFrameworkCore.Queries;
using Raiqub.Expressions.EntityFrameworkCore.Tests.Examples;
using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.EntityFrameworkCore.Tests.Queries;

public sealed class EFQueryTest : SqliteTestBase<BloggingContext>
{
    public EFQueryTest()
        : base(options => new BloggingContext(options))
    {
        DbContext.AddRange(GetBlogs());

        DbContext.SaveChanges();
#if NET5_0_OR_GREATER
        DbContext.ChangeTracker.Clear();
#endif
    }

    [Theory]
    [InlineData("First")]
    [InlineData("Second")]
    public async Task AnyShouldReturnTrue(string name)
    {
        var dbQuery = CreateQuery(new GetBlogPostsQueryModel(name));

        bool exists = await dbQuery.AnyAsync();

        exists.Should().BeTrue();
    }

    [Theory]
    [InlineData("Third")]
    [InlineData("Fourth")]
    [InlineData("Zero")]
    [InlineData("Other")]
    public async Task AnyShouldReturnFalse(string name)
    {
        var dbQuery = CreateQuery(new GetBlogPostsQueryModel(name));

        bool exists = await dbQuery.AnyAsync();

        exists.Should().BeFalse();
    }

    [Fact]
    public async Task CountAllShouldReturn3()
    {
        var efQuery = new EFQuery<Post>(NullLogger.Instance, DbContext.Set<Post>());

        long count = await efQuery.CountAsync();

        count.Should().Be(3);
    }

    [Theory]
    [InlineData("First", 2L)]
    [InlineData("Second", 1L)]
    [InlineData("Third", 0L)]
    [InlineData("Fourth", 0L)]
    public async Task CountShouldReturnExpected(string name, long expected)
    {
        var efQuery = CreateQuery(new GetBlogPostsQueryModel(name));

        long count = await efQuery.CountAsync();

        count.Should().Be(expected);
    }

    [Theory]
    [InlineData("First", "Nice")]
    [InlineData("Second", "Thank you")]
    [InlineData("Third", null)]
    [InlineData("Fourth", null)]
    public async Task FirstOrDefaultShouldReturnExpected(string name, string? expected)
    {
        var efQuery = CreateQuery(new GetBlogPostsQueryModel(name));

        Post? post = await efQuery.FirstOrDefaultAsync();

        post?.Title.Should().Be(expected);
    }

    [Fact]
    public async Task ToListShouldReturnAll()
    {
        var efQuery = CreateQuery(QueryModel.Create((IQueryable<Blog> source) => source.SelectMany(b => b.Posts)));

        var posts = await efQuery.ToListAsync();

        posts.Should().HaveCount(3);
        posts.Select(p => p.Title).Should().BeEquivalentTo("Nice", "The worst", "Thank you");
    }

    [Fact]
    public async Task ToListShouldReturnExpected()
    {
        var efQuery = CreateQuery(
            QueryModel.Create(
                (IQueryable<Blog> source) => source
                    .SelectMany(b => b.Posts)
                    .Where(p => p.Content.StartsWith("You"))));

        var posts = await efQuery.ToListAsync();

        posts.Should().HaveCount(2);
        posts.Select(p => p.Title).Should().BeEquivalentTo("The worst", "Thank you");
    }

    [Theory]
    [InlineData("Second", "Thank you")]
    [InlineData("Third", null)]
    [InlineData("Fourth", null)]
    public async Task SingleOrDefaultShouldReturnExpected(string name, string? expected)
    {
        var efQuery = CreateQuery(new GetBlogPostsQueryModel(name));

        Post? post = await efQuery.SingleOrDefaultAsync();

        post?.Title.Should().Be(expected);
    }

    [Theory]
    [InlineData("First")]
    public async Task SingleOrDefaultShouldFail(string name)
    {
        var efQuery = CreateQuery(new GetBlogPostsQueryModel(name));

        await efQuery
            .Invoking(q => q.SingleOrDefaultAsync())
            .Should().ThrowExactlyAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ShouldFailWhenSourceIsNull()
    {
        var efQuery = new EFQuery<Blog>(NullLogger.Instance, null!);

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

    private EFQuery<Post> CreateQuery(IQueryModel<Blog, Post> queryModel) => new(
        NullLogger.Instance,
        DbContext.Set<Blog>().AsNoTracking().Apply(queryModel));

    private static IEnumerable<Blog> GetBlogs()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;

        var first = new Blog("First");
        first.AddPost(new Post("Nice", "Keep writing", now.AddMilliseconds(1)));
        first.AddPost(new Post("The worst", "You should quit writing", now.AddMilliseconds(2)));
        yield return first;

        var second = new Blog("Second");
        second.AddPost(new Post("Thank you", "You helped a lot", now.AddMilliseconds(1)));
        yield return second;

        yield return new Blog("Third");
    }
}
