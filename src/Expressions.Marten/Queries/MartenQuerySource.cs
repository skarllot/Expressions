using Marten;
using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.Marten.Queries;

public class MartenQuerySource : IQuerySource
{
    private readonly IQuerySession _session;

    public MartenQuerySource(IQuerySession session) => _session = session;

    public IQueryable<TEntity> GetSet<TEntity>() where TEntity : class => _session.Query<TEntity>();
}
