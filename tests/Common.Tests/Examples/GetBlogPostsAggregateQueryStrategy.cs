﻿using Raiqub.Expressions;
using Raiqub.Expressions.Queries;

namespace Raiqub.Common.Tests.Examples;

public class GetBlogPostsAggregateQueryStrategy : EntityQueryStrategy<Blog, Post>
{
    public GetBlogPostsAggregateQueryStrategy(string name) => Name = name;

    public string Name { get; }

    protected override IQueryable<Post> ExecuteCore(IQueryable<Blog> source) => source
        .Where(BlogSpecification.OfName(Name))
        .SelectMany(b => b.Posts);
}
