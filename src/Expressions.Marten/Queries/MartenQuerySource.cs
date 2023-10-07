using Marten;
using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.Marten.Queries;

/// <summary>Marten-based implementation of provider of data sources.</summary>
public class MartenQuerySource : IQuerySource
{
    private readonly IQuerySession _session;

    /// <summary>Initializes a new instance of the <see cref="MartenQuerySource"/> class.</summary>
    /// <param name="session">The Marten session to query from.</param>
    public MartenQuerySource(IQuerySession session) => _session = session;

    /// <inheritdoc />
    public IQueryable<TEntity> GetSet<TEntity>() where TEntity : class => _session.Query<TEntity>();
}
