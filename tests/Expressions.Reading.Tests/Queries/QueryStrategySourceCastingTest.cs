using AwesomeAssertions;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Reading.Tests.Examples;

namespace Raiqub.Expressions.Reading.Tests.Queries;

public class QueryStrategySourceCastingTest
{
    [Fact]
    public void GivenABaseQueryStrategyWhenCastToDerivedThenReturnsInstance()
    {
        var list = new[]
        {
            new NewsPost(1, "First", "The first post", "general", "John"),
            new NewsPost(2, "Second", "The second post", "speech", "Carl")
        };

        var originalQueryStrategy = new BlogPostContentSearchQueryStrategy("second");
        var sourceCastQueryStrategy = originalQueryStrategy.SourceCast().Of<NewsPost>();

        var result1 = list
            .AsQueryable()
            .Apply(sourceCastQueryStrategy)
            .ToList();

        var result2 = list
            .Apply(sourceCastQueryStrategy)
            .ToList();

        var result3 = list
            .AsQueryable()
            .Apply(originalQueryStrategy)
            .ToList();

        result1.Should().ContainSingle(p => p.Id == 2);
        result2.Should().ContainSingle(p => p.Id == 2);
        result3.Should().ContainSingle(p => p.Id == 2);
    }
}
