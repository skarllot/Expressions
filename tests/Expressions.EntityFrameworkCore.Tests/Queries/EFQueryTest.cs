using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Raiqub.Common.Tests.Examples;
using Raiqub.Common.Tests.Queries;
using Raiqub.Expressions.EntityFrameworkCore.Queries;
using Raiqub.Expressions.EntityFrameworkCore.Tests.Examples;

namespace Raiqub.Expressions.EntityFrameworkCore.Tests.Queries;

public sealed class EFQueryTest : QueryTestBase
{
    public EFQueryTest()
        : base(
            services => services
                .AddSingleton<ILoggerFactory>(new NullLoggerFactory())
                .AddSqliteDbContext<BloggingContext>()
                .AddEntityFrameworkExpressions()
                .AddSingleContext<BloggingContext>())
    {
        ServiceProvider.GetRequiredService<BloggingContext>().Database.EnsureCreated();
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
}
