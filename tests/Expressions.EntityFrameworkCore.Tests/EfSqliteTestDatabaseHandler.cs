using Microsoft.EntityFrameworkCore;
using Raiqub.Common.Tests;

namespace Raiqub.Expressions.EntityFrameworkCore.Tests;

public sealed class EfSqliteTestDatabaseHandler<TContext> : ITestDatabaseHandler
    where TContext : DbContext
{
    private readonly SqliteTestDatabaseHandler _handler;

    public EfSqliteTestDatabaseHandler(Func<DbContextOptions<TContext>, TContext> contextBuilder)
    {
        _handler = new SqliteTestDatabaseHandler();

        var contextOptions = new DbContextOptionsBuilder<TContext>()
            .UseSqlite(_handler.Connection)
            .Options;

        DbContext = contextBuilder(contextOptions);
        DbContext.Database.EnsureCreated();
    }

    public TContext DbContext { get; }

    public void Dispose()
    {
        DbContext.Dispose();
        _handler.Dispose();
    }
}
