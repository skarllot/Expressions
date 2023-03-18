namespace Raiqub.Expressions.EntityFrameworkCore.Tests.Examples;

public class Post
{
    public Post(string title, string content)
    {
        Title = title;
        Content = content;
    }

    public string Title { get; protected set; }
    public string Content { get; protected set; }
}
