using FluentAssertions;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Reading.Tests.Examples;

namespace Raiqub.Expressions.Reading.Tests.Queries;

public class QueryStrategyTest
{
    [Fact]
    public void CreateShouldAlwaysReturnAll()
    {
        string[] source = { "john", "jane", "", "hugo", "jack" };
        var queryStrategy = QueryStrategy.AllOfEntity<string>();

        string?[] result1 = source
            .Apply(queryStrategy)
            .ToArray();
        string?[] result2 = source
            .AsQueryable()
            .Apply(queryStrategy)
            .ToArray();

        result1.Should().Equal("john", "jane", "", "hugo", "jack");
        result2.Should().Equal("john", "jane", "", "hugo", "jack");
    }

    [Fact]
    public void CreateShouldEvaluateSpecificationCorrectly()
    {
        string[] source = { "john", "jane", "hugo", "jack" };
        var queryStrategy = QueryStrategy.CreateForEntity(new StringBeginsWithJohnSpecification());

        string[] result1 = source
            .Apply(queryStrategy)
            .ToArray();
        string[] result2 = source
            .AsQueryable()
            .Apply(queryStrategy)
            .ToArray();

        result1.Should().Equal("john");
        result2.Should().Equal("john");
    }
}
