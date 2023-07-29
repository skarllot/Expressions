using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Raiqub.Common.Tests;

namespace Raiqub.Expressions.EntityFrameworkCore.Tests;

public static class DatabaseServiceCollectionExtensions
{
    public static IServiceCollection AddSqliteDbContext<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        services.AddSqliteConnection();

        services.TryAddScoped<DbContextOptions<TContext>>(
            static sp => new DbContextOptionsBuilder<TContext>()
                .UseSqlite(sp.GetRequiredService<SqliteConnection>())
                .Options);

        services.AddDbContext<TContext>();

        return services;
    }
}
