namespace Raiqub.Common.Tests.Examples;

public class GetBlogPostsQueryStrategy : GetBlogPostsAggregateQueryStrategy
{
    public GetBlogPostsQueryStrategy(string name) : base(name)
    {
    }

    protected override IQueryable<Post> ExecuteCore(IQueryable<Blog> source) => base
        .ExecuteCore(source)
        .OrderBy(p => p.Timestamp);
}
