using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.Reading.Tests.Examples;

public class NewsPostContentSearchQueryStrategy : EntityQueryStrategy<NewsPost, NewsPost>
{
    public NewsPostContentSearchQueryStrategy(string search) => Search = search;

    public string Search { get; }

    protected override IEnumerable<Specification<NewsPost>> GetPreconditions()
    {
        yield return new BlogPostContentSearchSpecification(Search).CastDown<NewsPost>();
    }

    protected override IQueryable<NewsPost> ExecuteCore(IQueryable<NewsPost> source) => source;
}
