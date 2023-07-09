using Marten;
using Microsoft.Extensions.Logging.Abstractions;
using Raiqub.Common.Tests.Examples;
using Raiqub.Common.Tests.Queries;
using Raiqub.Expressions.Marten.Queries;
using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.Marten.Tests.Queries;

public sealed class MartenQueryTest : QueryTestBase, IDisposable
{
    private readonly MartenPostgresTestDatabaseHandler _databaseHandler;

    public MartenQueryTest()
    {
        _databaseHandler = new MartenPostgresTestDatabaseHandler();
    }

    public void Dispose() => _databaseHandler.Dispose();

    protected override IQuery<Post> CreateQuery(IQueryModel<Blog, Post>? queryModel) => queryModel is not null
        ? new MartenQuery<Post>(
            NullLogger.Instance,
            _databaseHandler.Store.QuerySession().Query<Blog>().Apply(queryModel))
        : new MartenQuery<Post>(NullLogger.Instance, _databaseHandler.Store.QuerySession().Query<Post>());

    protected override async Task AddBlogs(IEnumerable<Blog> blogs)
    {
        await using IDocumentSession session = _databaseHandler.Store.LightweightSession();
        session.Insert(blogs);
        await session.SaveChangesAsync();
    }
}
