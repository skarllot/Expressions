using FluentAssertions;
using NSubstitute;
using Raiqub.Expressions.Queries;
using Raiqub.Expressions.Reading.Tests.Examples;

namespace Raiqub.Expressions.Reading.Tests.Queries;

public class QuerySourceExtensionsTest
{
    [Fact]
    public void GetSetUsingEntityQueryStrategyShouldReturnExpected()
    {
        string[] source = { "john", "jane", "hugo", "jack" };
        var queryStrategy = QueryStrategy.CreateForEntity(new StringBeginsWithJohnSpecification());

        IQuerySource querySource = Substitute.For<IQuerySource>();
        querySource.GetSet<string>().Returns(source.AsQueryable());

        var result = querySource.GetSetUsing(queryStrategy).ToList();

        result.Should().Equal("john");
    }

    [Fact]
    public void GetSetUsingSpecificationShouldReturnExpected()
    {
        string[] source = { "john", "jane", "hugo", "jack" };

        IQuerySource querySource = Substitute.For<IQuerySource>();
        querySource.GetSet<string>().Returns(source.AsQueryable());

        var result = querySource.GetSetUsing(new StringBeginsWithJohnSpecification()).ToList();

        result.Should().Equal("john");
    }
}
