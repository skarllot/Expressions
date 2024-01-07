using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.Reading.Tests.Examples;

public class NewsPostThatContainsFirstQueryStrategy : EntityQueryStrategy<NewsPost, BlogPost>
{
    protected override IQueryable<BlogPost> ExecuteCore(IQueryable<NewsPost> source) => source.Where(
        p => p.Content.Contains("first", StringComparison.InvariantCultureIgnoreCase));
}
