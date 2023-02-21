using FluentAssertions;

namespace Raiqub.Expressions.Tests;

public class SpecificationOfTTest
{
    [Theory]
    [InlineData((string?)null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("John")]
    public void AllShouldAlwaysReturnTrue(string input)
    {
        var specification = Specification<string>.All;

        bool result = specification.IsSatisfiedBy(input);
        result.Should().BeTrue();
    }

    [Fact]
    public void ToStringShouldReturnExpectedExpression()
    {
        var specification = Specification.Create<string>(x => x.Length > 0);

        string result = specification.ToString();
        result.Should().Be("x => (x.Length > 0)");
    }
}
