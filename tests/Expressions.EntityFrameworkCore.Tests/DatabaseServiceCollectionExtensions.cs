using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Raiqub.Expressions.EntityFrameworkCore.Tests;

public static class DatabaseServiceCollectionExtensions
{
    public static IServiceCollection AddPostgreSqlDbContext<TContext>(
        this IServiceCollection services,
        string connectionString)
        where TContext : DbContext
    {
        services.TryAddScoped<DbContextOptions<TContext>>(
            _ => new DbContextOptionsBuilder<TContext>()
                .UseNpgsql(connectionString)
                .Options);

        services.AddDbContextFactory<TContext>();

        return services;
    }
}
