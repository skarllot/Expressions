using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Raiqub.Expressions.EntityFrameworkCore.Tests;

public static class DatabaseServiceCollectionExtensions
{
    public static IServiceCollection AddPostgreSqlDbContext<TContext>(
        this IServiceCollection services,
        ITestOutputHelper testOutputHelper,
        string connectionString)
        where TContext : DbContext
    {
        services.TryAddScoped<DbContextOptions<TContext>>(
            _ => new DbContextOptionsBuilder<TContext>()
                .UseNpgsql(connectionString)
                .EnableSensitiveDataLogging()
                .LogTo(testOutputHelper.WriteLine, LogLevel.Information)
                .Options);

        services.AddDbContextFactory<TContext>();

        return services;
    }
}
