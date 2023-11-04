using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.Reading.Tests.Examples;

public class BlogPostContentSearchQueryStrategy : EntityQueryStrategy<BlogPost>
{
    public BlogPostContentSearchQueryStrategy(string search) => Search = search;

    public string Search { get; }

    protected override IEnumerable<Specification<BlogPost>> GetPreconditions()
    {
        yield return new BlogPostContentSearchSpecification(Search);
    }
}
