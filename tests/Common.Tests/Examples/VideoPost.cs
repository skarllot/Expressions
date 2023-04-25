namespace Raiqub.Common.Tests.Examples;

public class VideoPost : Post
{
    public VideoPost(string title, string content, string videoUrl, DateTimeOffset timestamp)
        : base(title, content, timestamp)
    {
        VideoUrl = videoUrl;
    }

    public string VideoUrl { get; protected set; }
}
