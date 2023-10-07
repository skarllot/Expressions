using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.EntityFrameworkCore.Options;
using Raiqub.Expressions.EntityFrameworkCore.Queries;
using Raiqub.Expressions.Sessions;
using Raiqub.Expressions.Sessions.BoundedContext;

namespace Raiqub.Expressions.EntityFrameworkCore.Sessions;

/// <summary>Entity Framework-based implementation of a factory for creating database sessions for data access.</summary>
public class EfDbSessionFactory<TContext>
    : IDbSessionFactory<TContext>, IDbQuerySessionFactory<TContext>, IDbSessionFactory, IDbQuerySessionFactory
    where TContext : DbContext
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly IDbContextFactory<TContext> _contextFactory;
    private readonly ISqlProviderSelector _sqlProviderSelector;
    private readonly EntityOptionsSelector _optionsSelector;

    /// <summary>Initializes a new instance of the <see cref="EfDbSessionFactory{TContext}"/> class.</summary>
    /// <param name="loggerFactory">The factory used to create loggers.</param>
    /// <param name="contextFactory">The factory for creating <see cref="DbContext"/> instances.</param>
    /// <param name="sqlProviderSelector">A selector to retrieve custom SQL for querying entities.</param>
    /// <param name="optionsSelector">A selector to retrieve options for handling entities.</param>
    public EfDbSessionFactory(
        ILoggerFactory loggerFactory,
        IDbContextFactory<TContext> contextFactory,
        ISqlProviderSelector sqlProviderSelector,
        EntityOptionsSelector optionsSelector)
    {
        _loggerFactory = loggerFactory;
        _contextFactory = contextFactory;
        _sqlProviderSelector = sqlProviderSelector;
        _optionsSelector = optionsSelector;
    }

    /// <summary>Creates a new database session.</summary>
    /// <param name="tracking">The change tracking mode of the new session.</param>
    /// <returns>A new database session.</returns>
    public EfDbSession<TContext> Create(ChangeTracking? tracking = null) => CreateWithContext(tracking);

    /// <summary>Creates a new database query session.</summary>
    /// <returns>A new database query session.</returns>
    public EfDbSession<TContext> CreateForQuery() => Create(ChangeTracking.Disable);

    IDbSession IDbSessionFactory.Create(ChangeTracking? tracking) => CreateWithoutContext(tracking);

    IDbSession<TContext> IDbSessionFactory<TContext>.Create(ChangeTracking? tracking) => CreateWithContext(tracking);

    IDbQuerySession IDbQuerySessionFactory.Create() => CreateWithoutContext(ChangeTracking.Disable);

    IDbQuerySession<TContext> IDbQuerySessionFactory<TContext>.Create() => CreateWithContext(ChangeTracking.Disable);

    private EfDbSession<TContext> CreateWithContext(ChangeTracking? tracking = null) => new(
        _loggerFactory.CreateLogger<EfDbSession<TContext>>(),
        _contextFactory.CreateDbContext(),
        _sqlProviderSelector,
        _optionsSelector,
        tracking ?? ChangeTracking.Default);

    private EfDbSession CreateWithoutContext(ChangeTracking? tracking = null) => new(
        _loggerFactory.CreateLogger<EfDbSession<TContext>>(),
        _contextFactory.CreateDbContext(),
        _sqlProviderSelector,
        _optionsSelector,
        tracking ?? ChangeTracking.Default);
}
