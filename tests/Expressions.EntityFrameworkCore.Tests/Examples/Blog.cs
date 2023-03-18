﻿namespace Raiqub.Expressions.EntityFrameworkCore.Tests.Examples;

public class Blog
{
    private readonly List<Post> _posts;

    public Blog(string name)
        : this(Guid.Empty, name)
    {
    }

    protected Blog(Guid id, string name)
    {
        Id = id;
        Name = name;
        _posts = new List<Post>();
    }

    public Guid Id { get; protected set; }
    public string Name { get; protected set; }

    public IReadOnlyList<Post> Posts => _posts;

    public void AddPost(Post post) => _posts.Add(post);
}
