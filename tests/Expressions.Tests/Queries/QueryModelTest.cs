using FluentAssertions;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Tests.Examples;

namespace Raiqub.Expressions.Tests.Queries;

public class QueryModelTest
{
    [Fact]
    public void CreateShouldAlwaysReturnAll()
    {
        var queryModel = QueryModel.Create<string?>();

        string?[] result = new[] { "john", "jane", null, "hugo", "jack" }
            .Apply(queryModel)
            .ToArray();

        result.Should().Equal("john", "jane", null, "hugo", "jack");
    }

    [Fact]
    public void CreateShouldEvaluateSpecificationCorrectly()
    {
        var queryModel = QueryModel.Create(new StringBeginsWithJohnSpecification());

        string[] result = new[] { "john", "jane", "hugo", "jack" }
            .Apply(queryModel)
            .ToArray();

        result.Should().Equal("john");
    }
}
