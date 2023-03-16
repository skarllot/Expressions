using Marten;
using Microsoft.Extensions.Logging;
using Raiqub.Expressions.Marten.Sessions;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Repositories;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Expressions.Marten.Repositories;

public class MartenReadRepository<TContext, TEntity> : IReadRepository<TContext, TEntity>
    where TContext : class, IDocumentStore
{
    private readonly ILogger<MartenReadRepository<TContext, TEntity>> _logger;
    private readonly TContext _context;

    public MartenReadRepository(ILogger<MartenReadRepository<TContext, TEntity>> logger, TContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IQuery<TResult> Query<TResult>(
        QueryModel<TEntity, TResult> queryModel,
        IReadSession<TContext>? session = null)
    {
        return new MartenQuery<TEntity, TResult>(
            _logger,
            _context,
            (session as MartenReadSession<TContext>)?.Session ?? _context.QuerySession(),
            queryModel);
    }
}
