using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MysticMind.PostgresEmbed;
using Npgsql;
using Raiqub.Common.Tests.Commons;

namespace Raiqub.Common.Tests;

public static class DatabaseServiceCollectionExtensions
{
    public static IServiceCollection AddSqliteConnection(this IServiceCollection services)
    {
        services.TryAddSingleton(
            static _ =>
            {
                var connection = new SqliteConnection("Filename=:memory:");
                connection.Open();
                return connection;
            });

        return services;
    }

    public static IServiceCollection AddPostgreSqlServer(this IServiceCollection services)
    {
        services.TryAddSingleton(_ => new PgServer(pgVersion: "10.7.1"));

        services.TryAddSingleton<NpgsqlConnectionStringBuilder>(
            sp =>
            {
                var pgServer = sp.GetRequiredService<PgServer>();
                pgServer.Start();
                PgIsReady.Wait(pgServer.GetConnectionString());

                return new NpgsqlConnectionStringBuilder(pgServer.GetConnectionString());
            });

        return services;
    }
}
