namespace Raiqub.Common.Tests.Examples;

public class Blog
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }

    public List<Post> Posts { get; set; } = [];
}
