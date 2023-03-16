using Marten;
using Raiqub.Expressions.Sessions;

namespace Raiqub.Expressions.Marten.Sessions;

public class MartenSessionFactory<TContext> : ISessionFactory<TContext>
    where TContext : IDocumentStore
{
    private readonly TContext _context;

    public MartenSessionFactory(TContext context)
    {
        _context = context;
    }

    public ISession<TContext> Create(ChangeTracking? tracking = null) =>
        new MartenSession<TContext>(_context, tracking);

    public IReadSession<TContext> CreateForQuery() =>
        new MartenReadSession<TContext>(_context);
}
