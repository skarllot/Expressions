using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Raiqub.Common.Tests.Examples;
using Raiqub.Common.Tests.Queries;

namespace Raiqub.Expressions.Marten.Tests.Queries;

[Collection("Marten")]
public sealed class MartenQueryTest : QueryTestBase
{
    public MartenQueryTest()
        : base(
            services => services
                .AddSingleton<ILoggerFactory>(new NullLoggerFactory())
                .AddTestMarten<Blog>()
                .AddMartenExpressions()
                .AddSingleContext())
    {
    }
}
