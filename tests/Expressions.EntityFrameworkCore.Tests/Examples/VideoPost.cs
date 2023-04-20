namespace Raiqub.Expressions.EntityFrameworkCore.Tests.Examples;

public class VideoPost : Post
{
    public VideoPost(string title, string content, string videoUrl)
        : base(title, content)
    {
        VideoUrl = videoUrl;
    }

    public string VideoUrl { get; protected set; }
}
