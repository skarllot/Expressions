using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Raiqub.Common.Tests.Examples;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Common.Tests.Sessions;

public abstract class SessionFactoryTestBase : DatabaseTestBase
{
    protected SessionFactoryTestBase(Action<IServiceCollection> registerServices)
        : base(registerServices)
    {
    }

    [Fact]
    public async Task AddWithoutSavingShouldNotCommitChanges()
    {
        var sessionFactory = CreateSessionFactory();

        await using (var session = sessionFactory.Create())
        {
            await session.AddAsync(GetBlogs().First());
        }

        int count;
        await using (var session = sessionFactory.Create())
        {
            count = await session.Query<Blog>().CountAsync();
        }

        count.Should().Be(0);
    }

    [Fact]
    public async Task AddAndSaveShouldCommitChanges()
    {
        var sessionFactory = CreateSessionFactory();

        await using (var session = sessionFactory.Create())
        {
            await session.AddAsync(GetBlogs().First());
            await session.SaveChangesAsync();
        }

        int count;
        await using (var session = sessionFactory.Create())
        {
            count = await session.Query<Blog>().CountAsync();
        }

        count.Should().Be(1);
    }

    [Fact]
    public async Task AddRangeAndSaveShouldCommitChanges()
    {
        var sessionFactory = CreateSessionFactory();

        await using (var session = sessionFactory.Create())
        {
            await session.AddRangeAsync(GetBlogs());
            await session.SaveChangesAsync();
        }

        int count;
        await using (var session = sessionFactory.Create())
        {
            count = await session.Query<Blog>().CountAsync();
        }

        count.Should().Be(GetBlogs().Count());
    }

    [Fact]
    public async Task UpdateAndSaveShouldCommitChanges()
    {
        var sessionFactory = CreateSessionFactory();

        await using (var session = sessionFactory.Create())
        {
            await session.AddRangeAsync(GetBlogs());
            await session.SaveChangesAsync();
        }

        await using (var session = sessionFactory.Create())
        {
            Blog blog = await session.Query(new GetBlogByNameQueryModel("Second")).FirstAsync();
            blog.AddPost(new Post("Test", "This is a test", DateTimeOffset.UtcNow));

            session.Update(blog);
            await session.SaveChangesAsync();
        }

        Blog finalBlog;
        await using (var session = sessionFactory.Create())
        {
            finalBlog = await session.Query(new GetBlogByNameQueryModel("Second")).FirstAsync();
        }

        finalBlog.Posts.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateRangeAndSaveShouldCommitChanges()
    {
        var sessionFactory = CreateSessionFactory();

        await using (var session = sessionFactory.Create())
        {
            await session.AddRangeAsync(GetBlogs());
            await session.SaveChangesAsync();
        }

        await using (var session = sessionFactory.Create())
        {
            var blogs = await session.Query<Blog>().ToListAsync();
            foreach (Blog blog in blogs)
            {
                blog.AddPost(new Post("Test", "This is a test", DateTimeOffset.UtcNow));
            }

            session.UpdateRange(blogs);
            await session.SaveChangesAsync();
        }

        IReadOnlyList<Blog> finalBlogs;
        await using (var session = sessionFactory.Create())
        {
            finalBlogs = await session
                .Query(EntityQueryModel.Create((IQueryable<Blog> source) => source.OrderBy(b => b.Name)))
                .ToListAsync();
        }

        finalBlogs.Should().HaveCount(3);
        finalBlogs[0].Posts.Should().HaveCount(3);
        finalBlogs[1].Posts.Should().HaveCount(2);
        finalBlogs[2].Posts.Should().HaveCount(1);
    }

    [Fact]
    public async Task RemoveAndSaveShouldCommitChanges()
    {
        var sessionFactory = CreateSessionFactory();

        await using (var session = sessionFactory.Create())
        {
            await session.AddRangeAsync(GetBlogs());
            await session.SaveChangesAsync();
        }

        await using (var session = sessionFactory.Create())
        {
            Blog blog = await session.Query(new GetBlogByNameQueryModel("Second")).FirstAsync();
            session.Remove(blog);
            await session.SaveChangesAsync();
        }

        IReadOnlyList<Blog> blogs;
        await using (var session = sessionFactory.Create())
        {
            blogs = await session.Query<Blog>().ToListAsync();
        }

        blogs.Should().NotContain(blog => blog.Name == "Second");
    }

    private IDbSessionFactory CreateSessionFactory() => ServiceProvider.GetRequiredService<IDbSessionFactory>();

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
