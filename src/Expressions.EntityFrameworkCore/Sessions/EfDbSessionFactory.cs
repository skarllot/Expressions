using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.EntityFrameworkCore.Options;
using Raiqub.Expressions.EntityFrameworkCore.Queries;
using Raiqub.Expressions.Sessions;
using Raiqub.Expressions.Sessions.BoundedContext;

namespace Raiqub.Expressions.EntityFrameworkCore.Sessions;

public class EfDbSessionFactory<TContext>
    : IDbSessionFactory<TContext>, IDbQuerySessionFactory<TContext>, IDbSessionFactory, IDbQuerySessionFactory
    where TContext : DbContext
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly IDbContextFactory<TContext> _contextFactory;
    private readonly ISqlProviderSelector _sqlProviderSelector;
    private readonly EntityOptionsSelector _optionsSelector;

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

    public EfDbSession<TContext> Create(ChangeTracking? tracking = null) => CreateWithContext(tracking);

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
