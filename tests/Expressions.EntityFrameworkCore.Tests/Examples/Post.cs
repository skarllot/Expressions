namespace Raiqub.Expressions.EntityFrameworkCore.Tests.Examples;

public class Post
{
    public Post(string title, string content, DateTimeOffset timestamp)
    {
        Title = title;
        Content = content;
        Timestamp = timestamp;
    }

    public string Title { get; protected set; }
    public string Content { get; protected set; }
    public DateTimeOffset Timestamp { get; protected set; }
}
