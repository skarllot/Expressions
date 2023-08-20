namespace Raiqub.Expressions.Reading.Tests.Examples;

public class BlogPost
{
    public BlogPost(int id, string title, string content, string postType)
    {
        Id = id;
        Title = title;
        Content = content;
        PostType = postType;
    }

    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string PostType { get; set; } // discriminator column
}

public class NewsPost : BlogPost
{
    public NewsPost(int id, string title, string content, string postType, string author)
        : base(id, title, content, postType)
    {
        Author = author;
    }

    public string Author { get; set; }
}

public class VideoPost : BlogPost
{
    public VideoPost(int id, string title, string content, string postType, string videoUrl)
        : base(id, title, content, postType)
    {
        VideoUrl = videoUrl;
    }

    public string VideoUrl { get; set; }
}
