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
            Blog blog = await session.Query(new GetBlogByNameQueryStrategy("Second")).FirstAsync();
            blog.Posts.Add(new Post { Title = "Test", Content = "This is a test", Timestamp = DateTimeOffset.UtcNow });

            session.Update(blog);
            await session.SaveChangesAsync();
        }

        Blog finalBlog;
        await using (var session = sessionFactory.Create())
        {
            finalBlog = await session.Query(new GetBlogByNameQueryStrategy("Second")).FirstAsync();
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
                blog.Posts.Add(new Post { Title = "Test", Content = "This is a test", Timestamp = DateTimeOffset.UtcNow });
            }

            session.UpdateRange(blogs);
            await session.SaveChangesAsync();
        }

        IReadOnlyList<Blog> finalBlogs;
        await using (var session = sessionFactory.Create())
        {
            finalBlogs = await session
                .Query(QueryStrategy.CreateForEntity((IQueryable<Blog> source) => source.OrderBy(b => b.Name)))
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
            Blog blog = await session.Query(new GetBlogByNameQueryStrategy("Second")).FirstAsync();
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

    [Fact]
    public async Task QueryUsingSpecificationShouldReturnExpected()
    {
        var sessionFactory = CreateSessionFactory();

        await using (var session = sessionFactory.Create())
        {
            await session.AddRangeAsync(GetBlogs());
            await session.SaveChangesAsync();
        }

        IReadOnlyList<Blog> blogs;
        await using (var session = sessionFactory.Create())
        {
            blogs = await session.Query(BlogSpecification.OfName("Second")).ToListAsync();
        }

        blogs.Should().HaveCount(1);
        blogs[0].Name.Should().Be("Second");
        blogs[0].Posts.Should().HaveCount(1);
    }

    [Fact]
    public async Task QueryUsingQueryStrategyShouldReturnExpected()
    {
        var queryStrategy = QueryStrategy.Create(
            source => from blog in source.GetSet<Blog>()
                select blog.Name);

        var sessionFactory = CreateSessionFactory();

        await using (var session = sessionFactory.Create())
        {
            await session.AddRangeAsync(GetBlogs());
            await session.SaveChangesAsync();
        }

        IReadOnlyList<string> blogs;
        await using (var session = sessionFactory.Create())
        {
            blogs = await session.Query(queryStrategy).ToListAsync();
        }

        blogs.Should().Equal("First", "Second", "Third");
    }

    protected IDbSessionFactory CreateSessionFactory() => ServiceProvider.GetRequiredService<IDbSessionFactory>();

    protected static IEnumerable<Blog> GetBlogs()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;

        var first = new Blog { Id = new Guid("018a7015-fd5b-48a2-9ffa-07ef1ce7486d"), Name = "First" };
        first.Posts.Add(new Post { Title = "Nice", Content = "Keep writing", Timestamp = now.AddMilliseconds(1) });
        first.Posts.Add(new Post { Title = "The worst", Content = "You should quit writing", Timestamp = now.AddMilliseconds(2) });
        yield return first;

        var second = new Blog { Id = new Guid("018a7016-05a4-48c3-8545-63549cd3aeed"), Name = "Second" };
        second.Posts.Add(new Post { Title = "Thank you", Content = "You helped a lot", Timestamp = now.AddMilliseconds(1) });
        yield return second;

        yield return new Blog { Id = new Guid("018a7018-8fee-4acf-968b-5c89f5599f23"), Name = "Third" };
    }
}
