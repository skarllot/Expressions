using Marten;
using Microsoft.Extensions.DependencyInjection;
using Weasel.Core;

namespace Raiqub.Expressions.Marten.Tests;

public static class DatabaseServiceCollectionExtensions
{
    public static IServiceCollection AddTestMarten<TEntity>(this IServiceCollection services, string connectionString)
    {
        services.AddMarten(
            _ =>
            {
                var storeOptions = new StoreOptions();
                storeOptions.Connection(connectionString);
                storeOptions.AutoCreateSchemaObjects = AutoCreate.All;
                storeOptions.Schema.For<TEntity>();
                return storeOptions;
            });

        return services;
    }
}
