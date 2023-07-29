using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Raiqub.Common.Tests.Examples;
using Raiqub.Common.Tests.Sessions;

namespace Raiqub.Expressions.Marten.Tests.Sessions;

[Collection("Marten")]
public sealed class MartenSessionFactoryTest : SessionFactoryTestBase
{
    public MartenSessionFactoryTest()
        : base(
            services => services
                .AddSingleton<ILoggerFactory>(new NullLoggerFactory())
                .AddTestMarten<Blog>()
                .AddMartenExpressions()
                .AddSingleContext())
    {
    }
}
