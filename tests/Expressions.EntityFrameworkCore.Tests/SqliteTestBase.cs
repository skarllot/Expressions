using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Raiqub.Expressions.EntityFrameworkCore.Tests;

public class SqliteTestBase<TContext> : IDisposable
    where TContext : DbContext
{
    private readonly SqliteConnection _connection;

    public SqliteTestBase(Func<DbContextOptions<TContext>, TContext> contextBuilder)
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var contextOptions = new DbContextOptionsBuilder<TContext>()
            .UseSqlite(_connection)
            .Options;

        DbContext = contextBuilder(contextOptions);

        DbContext.Database.EnsureCreated();
    }

    public TContext DbContext { get; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            DbContext.Dispose();
            _connection.Dispose();
        }
    }
}
