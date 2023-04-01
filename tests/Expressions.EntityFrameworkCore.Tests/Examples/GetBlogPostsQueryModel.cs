﻿using Raiqub.Expressions.Queries;

namespace Raiqub.Expressions.EntityFrameworkCore.Tests.Examples;

public class GetBlogPostsQueryModel : QueryModel<Blog, Post>
{
    public GetBlogPostsQueryModel(string name) => Name = name;

    public string Name { get; }

    protected override IEnumerable<Specification<Blog>> GetPreconditions()
    {
        yield return BlogSpecification.OfName(Name);
    }

    protected override IQueryable<Post> ExecuteCore(IQueryable<Blog> source) => source
        .SelectMany(b => b.Posts);
}