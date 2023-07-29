using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

        _services.AddScoped<EFSessionFactory<TDbContext>>();
        _services.AddTransient<ISessionFactory>(sp => sp.GetRequiredService<EFSessionFactory<TDbContext>>());
        _services.AddTransient<IQuerySessionFactory>(sp => sp.GetRequiredService<EFSessionFactory<TDbContext>>());
        _services.AddScoped<ISession>(sp => sp.GetRequiredService<ISessionFactory>().Create(combinedTracking));
        _services.AddScoped<IQuerySession>(sp => sp.GetRequiredService<IQuerySessionFactory>().Create());
    }

    /// <summary>
    /// Register sessions and session factories for the given context.
    /// The context <typeparamref name="TContext"/> must be specified by dependent types (e.g. <see cref="ISession{TContext}"/>).
    /// </summary>
    /// <param name="tracking">The change tracking mode of injected sessions.</param>
    /// <typeparam name="TContext">The domain-level type of context to used by sessions.</typeparam>
    /// <typeparam name="TDbContext">The type of <see cref="DbContext"/> to be used by sessions.</typeparam>
    /// <returns>The current <see cref="ExpressionsSessionBuilder"/>.</returns>
    public ExpressionsSessionBuilder AddContext<TContext, TDbContext>(ChangeTracking? tracking = null)
        where TDbContext : DbContext, TContext
    {
        var combinedTracking = tracking ?? _tracking;

        _services.AddScoped<EFSessionFactory<TDbContext>>();
        _services.AddTransient<ISessionFactory<TContext>>(sp => sp.GetRequiredService<EFSessionFactory<TDbContext>>());
        _services.AddTransient<IQuerySessionFactory<TContext>>(
            sp => sp.GetRequiredService<EFSessionFactory<TDbContext>>());
        _services.AddScoped<ISession<TContext>>(
            sp => sp.GetRequiredService<ISessionFactory<TContext>>().Create(combinedTracking));
        _services.AddScoped<IQuerySession<TContext>>(
            sp => sp.GetRequiredService<IQuerySessionFactory<TContext>>().Create());

        return this;
    }
}
