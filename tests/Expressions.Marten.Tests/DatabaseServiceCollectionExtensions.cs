using Marten;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Raiqub.Common.Tests;
using Weasel.Core;

namespace Raiqub.Expressions.Marten.Tests;

public static class DatabaseServiceCollectionExtensions
{
    public static IServiceCollection AddTestMarten<TEntity>(this IServiceCollection services)
    {
        services.AddPostgreSqlServer();

        services.AddMarten(
            sp =>
            {
                var storeOptions = new StoreOptions();
                storeOptions.Connection(sp.GetRequiredService<NpgsqlConnectionStringBuilder>().ConnectionString);
                storeOptions.AutoCreateSchemaObjects = AutoCreate.All;
                storeOptions.Schema.For<TEntity>();
                return storeOptions;
            });

        return services;
    }
}
