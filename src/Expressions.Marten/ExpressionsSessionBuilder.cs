using Marten;
using Microsoft.Extensions.DependencyInjection;
using Raiqub.Expressions.Marten.Sessions;
using Raiqub.Expressions.Marten.Sessions.BoundedContext;
using Raiqub.Expressions.Sessions;
using Raiqub.Expressions.Sessions.BoundedContext;

namespace Raiqub.Expressions.Marten;

/// <summary>Used to configure the registration of sessions and sessions factories.</summary>
public sealed class ExpressionsSessionBuilder
{
    private readonly IServiceCollection _services;
    private readonly ChangeTracking? _tracking;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionsSessionBuilder"/> class.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="tracking">The change tracking mode of injected sessions.</param>
    public ExpressionsSessionBuilder(IServiceCollection services, ChangeTracking? tracking)
    {
        _services = services;
        _tracking = tracking;
    }

    /// <summary>
    /// Register sessions and session factories for the given context.
    /// The application has only one context.
    /// </summary>
    /// <param name="tracking">The change tracking mode of injected sessions.</param>
    public void AddSingleContext(ChangeTracking? tracking = null)
    {
        var combinedTracking = tracking ?? _tracking;

        _services.AddSingleton<MartenDbSessionFactory>();
        _services.AddSingleton<IDbSessionFactory>(sp => sp.GetRequiredService<MartenDbSessionFactory>());
        _services.AddSingleton<IDbQuerySessionFactory>(sp => sp.GetRequiredService<MartenDbSessionFactory>());

        if (combinedTracking is null)
            _services.AddScoped<IDbSession>(sp => sp.GetRequiredService<IDbSessionFactory>().Create());
        else
            _services.AddScoped<IDbSession>(sp => sp.GetRequiredService<IDbSessionFactory>().Create(combinedTracking));

        _services.AddScoped<IDbQuerySession>(sp => sp.GetRequiredService<IDbQuerySessionFactory>().Create());
    }

    /// <summary>
    /// Register sessions and session factories for the given context.
    /// The context <typeparamref name="TContext"/> must be specified by dependent types (e.g. <see cref="IDbSession{TContext}"/>).
    /// </summary>
    /// <param name="tracking">The change tracking mode of injected sessions.</param>
    /// <typeparam name="TContext">The domain-level type of context to used by sessions.</typeparam>
    /// <typeparam name="TDbContext">The type of <see cref="IDocumentStore"/> to be used by sessions.</typeparam>
    /// <returns>The current <see cref="ExpressionsSessionBuilder"/>.</returns>
    public ExpressionsSessionBuilder AddContext<TContext, TDbContext>(ChangeTracking? tracking = null)
        where TDbContext : class, IDocumentStore, TContext
    {
        var combinedTracking = tracking ?? _tracking;

        _services.AddSingleton<MartenDbSessionFactory<TDbContext>>();
        _services.AddSingleton<IDbSessionFactory<TContext>>(
            sp => sp.GetRequiredService<MartenDbSessionFactory<TDbContext>>());
        _services.AddSingleton<IDbQuerySessionFactory<TContext>>(
            sp => sp.GetRequiredService<MartenDbSessionFactory<TDbContext>>());

        if (combinedTracking is null)
            _services.AddScoped<IDbSession<TContext>>(
                sp => sp.GetRequiredService<IDbSessionFactory<TContext>>().Create());
        else
            _services.AddScoped<IDbSession<TContext>>(
                sp => sp.GetRequiredService<IDbSessionFactory<TContext>>().Create(combinedTracking));

        _services.AddScoped<IDbQuerySession<TContext>>(
            sp => sp.GetRequiredService<IDbQuerySessionFactory<TContext>>().Create());

        return this;
    }
}
