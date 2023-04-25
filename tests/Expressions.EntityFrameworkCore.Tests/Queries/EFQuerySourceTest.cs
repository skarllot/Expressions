using FluentAssertions;
using Raiqub.Common.Tests.Examples;
using Raiqub.Expressions.EntityFrameworkCore.Queries;
using Raiqub.Expressions.EntityFrameworkCore.Tests.Examples;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Expressions.EntityFrameworkCore.Tests.Queries;

public class EFQuerySourceTest : SqliteTestBase<BloggingContext>
{
    public EFQuerySourceTest()
        : base(options => new BloggingContext(options))
    {
    }

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
