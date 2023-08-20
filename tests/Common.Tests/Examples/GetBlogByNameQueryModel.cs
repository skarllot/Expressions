using Raiqub.Expressions;
using Raiqub.Expressions.Queries;

namespace Raiqub.Common.Tests.Examples;

public class GetBlogByNameQueryModel : EntityQueryModel<Blog, Blog>
{
    public GetBlogByNameQueryModel(string name) => Name = name;

    public string Name { get; }

    protected override IEnumerable<Specification<Blog>> GetPreconditions()
    {
        yield return BlogSpecification.OfName(Name);
    }

    protected override IQueryable<Blog> ExecuteCore(IQueryable<Blog> source) => source;
}
