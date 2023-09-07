using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Raiqub.Expressions.EntityFrameworkCore.Queries;
using Raiqub.Expressions.EntityFrameworkCore.Sessions;
using Raiqub.Expressions.Sessions;
using Raiqub.Expressions.Sessions.BoundedContext;

namespace Raiqub.Expressions.EntityFrameworkCore;

/// <summary>Used to configure registration of session and session factories.</summary>
public sealed class ExpressionsSessionBuilder
{
    private readonly IServiceCollection _services;
    private readonly ChangeTracking? _tracking;

    /// <summary>Creates a new instance of <see cref="ExpressionsSessionBuilder"/> class.</summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="tracking">The change tracking mode of injected sessions.</param>
    public ExpressionsSessionBuilder(IServiceCollection services, ChangeTracking? tracking)
    {
        _services = services;
        _tracking = tracking;
    }

    /// <summary>
    /// Register sessions and session factories for the given context.
    /// The context is the only context used by the application.
    /// </summary>
    /// <param name="tracking">The change tracking mode of injected sessions.</param>
    /// <typeparam name="TDbContext">The type of <see cref="DbContext"/> to be used for sessions.</typeparam>
    public void AddSingleContext<TDbContext>(ChangeTracking? tracking = null)
        where TDbContext : DbContext
    {
        var combinedTracking = tracking ?? _tracking;

        _services.AddSingleton<EfDbSessionFactory<TDbContext>>();
        _services.AddSingleton<IDbSessionFactory>(sp => sp.GetRequiredService<EfDbSessionFactory<TDbContext>>());
        _services.AddSingleton<IDbQuerySessionFactory>(sp => sp.GetRequiredService<EfDbSessionFactory<TDbContext>>());
        _services.TryAddSingleton<ISqlProviderSelector, SqlProviderSelector>();
        _services.AddScoped<IDbSession>(sp => sp.GetRequiredService<IDbSessionFactory>().Create(combinedTracking));
        _services.AddScoped<IDbQuerySession>(sp => sp.GetRequiredService<IDbQuerySessionFactory>().Create());
    }

    /// <summary>
    /// Register sessions and session factories for the given context.
    /// The context <typeparamref name="TContext"/> must be specified by dependent types (e.g. <see cref="IDbSession{TContext}"/>).
    /// </summary>
    /// <param name="tracking">The change tracking mode of injected sessions.</param>
    /// <typeparam name="TContext">The domain-level type of context to used by sessions.</typeparam>
    /// <typeparam name="TDbContext">The type of <see cref="DbContext"/> to be used by sessions.</typeparam>
    /// <returns>The current <see cref="ExpressionsSessionBuilder"/>.</returns>
    public ExpressionsSessionBuilder AddContext<TContext, TDbContext>(ChangeTracking? tracking = null)
        where TDbContext : DbContext, TContext
    {
        var combinedTracking = tracking ?? _tracking;

        _services.AddSingleton<EfDbSessionFactory<TDbContext>>();
        _services.AddSingleton<IDbSessionFactory<TContext>>(sp => sp.GetRequiredService<EfDbSessionFactory<TDbContext>>());
        _services.AddSingleton<IDbQuerySessionFactory<TContext>>(
            sp => sp.GetRequiredService<EfDbSessionFactory<TDbContext>>());
        _services.TryAddSingleton<ISqlProviderSelector, SqlProviderSelector>();
        _services.AddScoped<IDbSession<TContext>>(
            sp => sp.GetRequiredService<IDbSessionFactory<TContext>>().Create(combinedTracking));
        _services.AddScoped<IDbQuerySession<TContext>>(
            sp => sp.GetRequiredService<IDbQuerySessionFactory<TContext>>().Create());

        return this;
    }
}
