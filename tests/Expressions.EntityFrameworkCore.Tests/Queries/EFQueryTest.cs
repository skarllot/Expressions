using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Raiqub.Expressions.EntityFrameworkCore.Queries;
using Raiqub.Expressions.EntityFrameworkCore.Tests.Examples;

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
        var dbQuery = CreateQuery(name);

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
        var dbQuery = CreateQuery(name);

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
        var efQuery = CreateQuery(name);

        long count = await efQuery.CountAsync();

        count.Should().Be(expected);
    }

    [Theory]
    [InlineData("First", "The worst")]
    [InlineData("Second", "Thank you")]
    [InlineData("Third", null)]
    [InlineData("Fourth", null)]
    public async Task FirstOrDefaultShouldReturnExpected(string name, string? expected)
    {
        var efQuery = CreateQuery(name);

        Post? post = await efQuery.FirstOrDefaultAsync();

        post?.Title.Should().Be(expected);
    }

    private EFQuery<Post> CreateQuery(string name) => new(
        NullLogger.Instance,
        DbContext.Set<Blog>().AsNoTracking().Apply(new GetBlogPostsQueryModel(name)));

    private static IEnumerable<Blog> GetBlogs()
    {
        var first = new Blog("First");
        first.AddPost(new Post("Nice", "Keep writing"));
        first.AddPost(new Post("The worst", "You should quit writing"));
        yield return first;

        var second = new Blog("Second");
        second.AddPost(new Post("Thank you", "You helped a lot"));
        yield return second;

        yield return new Blog("Third");
    }
}
