using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Raiqub.Common.Tests.Sessions;
using Raiqub.Expressions.EntityFrameworkCore.Tests.Examples;

namespace Raiqub.Expressions.EntityFrameworkCore.Tests.Sessions;

public sealed class EFSessionFactoryTest : SessionFactoryTestBase
{
    public EFSessionFactoryTest()
        : base(
            services => services
                .AddSingleton<ILoggerFactory>(new NullLoggerFactory())
                .AddSqliteDbContext<BloggingContext>()
                .AddEntityFrameworkExpressions()
                .AddSingleContext<BloggingContext>())
    {
        ServiceProvider.GetRequiredService<BloggingContext>().Database.EnsureCreated();
    }
}
