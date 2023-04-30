using FluentAssertions;
using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Raiqub.Common.Tests.Examples;
using Raiqub.Expressions.Marten.Queries;
using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.Marten.Tests.Queries;

public class MartenQueryTest : PostgresTestBase
{
    [Theory]
    [InlineData("First")]
    [InlineData("Second")]
    public async Task AnyShouldReturnTrue(string name)
    {
        var dbQuery = CreateQuery(new GetBlogPostsQueryModel(name));

        bool exists = await dbQuery.AnyAsync();

        exists.Should().BeTrue();
    }

    private MartenQuery<Post> CreateQuery(IQueryModel<Blog, Post> queryModel) => new(
        NullLogger.Instance,
        Store.QuerySession().Query<Blog>().Apply(queryModel));

    protected override void InitializeData(IDocumentSession session)
    {
        session.Insert(GetBlogs());
    }

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
