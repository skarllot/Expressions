using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Raiqub.Expressions.EntityFrameworkCore.Queries;
using Raiqub.Expressions.EntityFrameworkCore.Tests.Examples;

namespace Raiqub.Expressions.EntityFrameworkCore.Tests.Queries;

public sealed class DbQueryTest : SqliteTestBase<BloggingContext>
{
    public DbQueryTest()
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
        var dbQuery = CreateDbQuery(name);

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
        var dbQuery = CreateDbQuery(name);

        bool exists = await dbQuery.AnyAsync();

        exists.Should().BeFalse();
    }

    private EFQuery<Post> CreateDbQuery(string name) => new(
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
