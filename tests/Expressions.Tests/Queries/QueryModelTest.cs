using FluentAssertions;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Tests.Examples;

namespace Raiqub.Expressions.Tests.Queries;

public class QueryModelTest
{
    [Fact]
    public void CreateShouldAlwaysReturnAll()
    {
        string?[] source = { "john", "jane", null, "hugo", "jack" };
        var queryModel = QueryModel.Create<string?>();

        string?[] result1 = source
            .Apply(queryModel)
            .ToArray();
        string?[] result2 = source
            .AsQueryable()
            .Apply(queryModel)
            .ToArray();

        result1.Should().Equal("john", "jane", null, "hugo", "jack");
        result2.Should().Equal("john", "jane", null, "hugo", "jack");
    }

    [Fact]
    public void CreateShouldEvaluateSpecificationCorrectly()
    {
        string[] source = { "john", "jane", "hugo", "jack" };
        var queryModel = QueryModel.Create(new StringBeginsWithJohnSpecification());

        string[] result1 = source
            .Apply(queryModel)
            .ToArray();
        string[] result2 = source
            .AsQueryable()
            .Apply(queryModel)
            .ToArray();

        result1.Should().Equal("john");
        result2.Should().Equal("john");
    }
}
