using Microsoft.Extensions.Logging.Abstractions;
using Raiqub.Common.Tests.Sessions;
using Raiqub.Expressions.EntityFrameworkCore.Sessions;
using Raiqub.Expressions.EntityFrameworkCore.Tests.Examples;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Expressions.EntityFrameworkCore.Tests.Sessions;

public sealed class EFSessionFactoryTest : SessionFactoryTestBase, IDisposable
{
    private readonly EfSqliteTestDatabaseHandler<BloggingContext> _databaseHandler;

    public EFSessionFactoryTest()
    {
        _databaseHandler = new EfSqliteTestDatabaseHandler<BloggingContext>(options => new BloggingContext(options));
    }

    public void Dispose() => _databaseHandler.Dispose();

    protected override ISessionFactory CreateSessionFactory()
    {
        return new EFSessionFactory<BloggingContext>(new NullLoggerFactory(), _databaseHandler.DbContext);
    }
}
