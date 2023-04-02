using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.Tests.Examples;

public class BlogPostContentSearchQueryModel : QueryModel<BlogPost>
{
    public BlogPostContentSearchQueryModel(string search) => Search = search;

    public string Search { get; }

    protected override IEnumerable<Specification<BlogPost>> GetPreconditions()
    {
        yield return new BlogPostContentSearchSpecification(Search);
    }

    protected override IQueryable<BlogPost> ExecuteCore(IQueryable<BlogPost> source) => source;
}
