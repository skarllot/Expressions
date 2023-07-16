using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Raiqub.Common.Tests.Examples;
using Raiqub.Common.Tests.Queries;
using Raiqub.Expressions.EntityFrameworkCore.Queries;
using Raiqub.Expressions.EntityFrameworkCore.Tests.Examples;
using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.EntityFrameworkCore.Tests.Queries;

public sealed class EFQueryTest : QueryTestBase, IDisposable
{
    private readonly EfSqliteTestDatabaseHandler<BloggingContext> _databaseHandler;

    public EFQueryTest()
    {
        _databaseHandler = new EfSqliteTestDatabaseHandler<BloggingContext>(options => new BloggingContext(options));
    }

    public void Dispose() => _databaseHandler.Dispose();

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

    protected override async Task AddBlogs(IEnumerable<Blog> blogs)
    {
        await _databaseHandler.DbContext.AddRangeAsync(blogs);
        await _databaseHandler.DbContext.SaveChangesAsync();
        _databaseHandler.DbContext.ChangeTracker.Clear();
    }

    protected override IQuery<Post> CreateQuery(IQueryModel<Blog, Post>? queryModel)
    {
        return queryModel is not null
            ? new EFQuery<Post>(
                NullLogger.Instance,
                _databaseHandler.DbContext.Set<Blog>().AsNoTracking().Apply(queryModel))
            : new EFQuery<Post>(NullLogger.Instance, _databaseHandler.DbContext.Set<Post>().AsNoTracking());
    }
}
