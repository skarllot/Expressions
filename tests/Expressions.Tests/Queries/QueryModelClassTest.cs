using FluentAssertions;
using Raiqub.Expressions.Tests.Examples;

namespace Raiqub.Expressions.Tests.Queries;

public class QueryModelClassTest
{
    [Fact]
    public void ShouldFilterAndTransformJohnDees()
    {
        (string Value, int Length)[] result1 = GetAll()
            .Apply(new JohnDeeQueryModel())
            .ToArray();
        (string Value, int Length)[] result2 = GetAll()
            .AsQueryable()
            .Apply(new JohnDeeQueryModel())
            .ToArray();

        result1.Should().HaveCount(3);
        result1.Should().Equal(("John Dee", 8), ("John Beacon Dee", 15), ("john moor dee", 13));
        result2.Should().HaveCount(3);
        result2.Should().Equal(("John Dee", 8), ("John Beacon Dee", 15), ("john moor dee", 13));
    }

    [Fact]
    public void ShouldFilterAndTransformJohnDeesWithMoor()
    {
        (string Value, int Length)[] result1 = GetAll()
            .Apply(new JohnDeeQueryModel(new StringContainsMoorSpecification()))
            .ToArray();
        (string Value, int Length)[] result2 = GetAll()
            .AsQueryable()
            .Apply(new JohnDeeQueryModel(new StringContainsMoorSpecification()))
            .ToArray();

        result1.Should().HaveCount(1);
        result1.Should().Equal(("john moor dee", 13));
        result2.Should().HaveCount(1);
        result2.Should().Equal(("john moor dee", 13));
    }

    [Fact]
    public void ShouldFilterWhenUsingCastDownSpecification()
    {
        var list = new[] { new NewsPost(1, "First", "The first post", "general", "John") };

        NewsPost[] result = list
            .Apply(new NewsPostContentSearchQueryModel("first"))
            .ToArray();

        result.Should().HaveCount(1);
    }

    private static IEnumerable<string> GetAll()
    {
        yield return "John Dee";
        yield return "John Dee Junior";
        yield return "John Beacon Dee";
        yield return "Jane Dee";
        yield return "John Moor";
        yield return "john moor dee";
    }
}
