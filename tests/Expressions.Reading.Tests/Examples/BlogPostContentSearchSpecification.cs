using System.Linq.Expressions;

namespace Raiqub.Expressions.Reading.Tests.Examples;

public class BlogPostContentSearchSpecification : Specification<BlogPost>
{
    public BlogPostContentSearchSpecification(string search) => Search = search;

    public string Search { get; }

    public override Expression<Func<BlogPost, bool>> ToExpression() => p => p.Content.Contains(Search);
}
