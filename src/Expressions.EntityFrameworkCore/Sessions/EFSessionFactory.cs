using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Sessions;
using Raiqub.Expressions.Sessions.BoundedContext;

namespace Raiqub.Expressions.EntityFrameworkCore.Sessions;

public class EFSessionFactory<TContext>
    : ISessionFactory<TContext>, IQuerySessionFactory<TContext>, ISessionFactory, IQuerySessionFactory
    where TContext : DbContext
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly IDbContextFactory<TContext> _contextFactory;

    public EFSessionFactory(ILoggerFactory loggerFactory, IDbContextFactory<TContext> contextFactory)
    {
        _loggerFactory = loggerFactory;
        _contextFactory = contextFactory;
    }

    public EFSession<TContext> Create(ChangeTracking? tracking = null) => new(
        _loggerFactory.CreateLogger<EFSession<TContext>>(),
        _contextFactory.CreateDbContext(),
        tracking ?? ChangeTracking.Default);

    public EFSession<TContext> CreateForQuery() => Create(ChangeTracking.Disable);

    ISession ISessionFactory.Create(ChangeTracking? tracking) => Create(tracking);

    ISession<TContext> ISessionFactory<TContext>.Create(ChangeTracking? tracking) => Create(tracking);

    IQuerySession IQuerySessionFactory.Create() => CreateForQuery();

    IQuerySession<TContext> IQuerySessionFactory<TContext>.Create() => CreateForQuery();
}
