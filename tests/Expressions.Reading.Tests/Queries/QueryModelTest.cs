using FluentAssertions;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Reading.Tests.Examples;

namespace Raiqub.Expressions.Reading.Tests.Queries;

public class QueryModelTest
{
    [Fact]
    public void CreateShouldAlwaysReturnAll()
    {
        string[] source = { "john", "jane", "", "hugo", "jack" };
        var queryModel = QueryModel.AllOfEntity<string>();

        string?[] result1 = source
            .Apply(queryModel)
            .ToArray();
        string?[] result2 = source
            .AsQueryable()
            .Apply(queryModel)
            .ToArray();

        result1.Should().Equal("john", "jane", "", "hugo", "jack");
        result2.Should().Equal("john", "jane", "", "hugo", "jack");
    }

    [Fact]
    public void CreateShouldEvaluateSpecificationCorrectly()
    {
        string[] source = { "john", "jane", "hugo", "jack" };
        var queryModel = QueryModel.CreateForEntity(new StringBeginsWithJohnSpecification());

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
