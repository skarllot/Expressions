using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.Tests.Examples;

public class NewsPostContentSearchQueryModel : QueryModel<NewsPost, NewsPost>
{
    public NewsPostContentSearchQueryModel(string search) => Search = search;

    public string Search { get; }

    protected override IEnumerable<Specification<NewsPost>> GetPreconditions()
    {
        yield return new BlogPostContentSearchSpecification(Search).CastDown<NewsPost>();
    }

    protected override IQueryable<NewsPost> ExecuteCore(IQueryable<NewsPost> source) => source;
}
