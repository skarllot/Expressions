using Marten;
using Microsoft.Extensions.DependencyInjection;
using Raiqub.Expressions.Marten.Sessions;
using Raiqub.Expressions.Marten.Sessions.BoundedContext;
using Raiqub.Expressions.Sessions;
using Raiqub.Expressions.Sessions.BoundedContext;
using IQuerySession = Raiqub.Expressions.Sessions.IQuerySession;
using ISessionFactory = Raiqub.Expressions.Sessions.ISessionFactory;

namespace Raiqub.Expressions.Marten;

public sealed class ExpressionsSessionBuilder
{
    private readonly IServiceCollection _services;
    private readonly ChangeTracking? _tracking;

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

        _services.AddSingleton<MartenSessionFactory>();
        _services.AddSingleton<ISessionFactory>(sp => sp.GetRequiredService<MartenSessionFactory>());
        _services.AddSingleton<IQuerySessionFactory>(sp => sp.GetRequiredService<MartenSessionFactory>());
        _services.AddScoped<ISession>(sp => sp.GetRequiredService<ISessionFactory>().Create(combinedTracking));
        _services.AddScoped<IQuerySession>(sp => sp.GetRequiredService<IQuerySessionFactory>().Create());
    }

    /// <summary>
    /// Register sessions and session factories for the given context.
    /// The context <typeparamref name="TContext"/> must be specified by dependent types (e.g. <see cref="ISession{TContext}"/>).
    /// </summary>
    /// <param name="tracking">The change tracking mode of injected sessions.</param>
    /// <typeparam name="TContext">The domain-level type of context to used by sessions.</typeparam>
    /// <typeparam name="TDbContext">The type of <see cref="IDocumentStore"/> to be used by sessions.</typeparam>
    /// <returns>The current <see cref="ExpressionsSessionBuilder"/>.</returns>
    public ExpressionsSessionBuilder AddContext<TContext, TDbContext>(ChangeTracking? tracking = null)
        where TDbContext : class, IDocumentStore, TContext
    {
        var combinedTracking = tracking ?? _tracking;

        _services.AddSingleton<MartenSessionFactory<TDbContext>>();
        _services.AddSingleton<ISessionFactory<TContext>>(sp => sp.GetRequiredService<MartenSessionFactory<TDbContext>>());
        _services.AddSingleton<IQuerySessionFactory<TContext>>(
            sp => sp.GetRequiredService<MartenSessionFactory<TDbContext>>());
        _services.AddScoped<ISession<TContext>>(
            sp => sp.GetRequiredService<ISessionFactory<TContext>>().Create(combinedTracking));
        _services.AddScoped<IQuerySession<TContext>>(
            sp => sp.GetRequiredService<IQuerySessionFactory<TContext>>().Create());

        return this;
    }
}
