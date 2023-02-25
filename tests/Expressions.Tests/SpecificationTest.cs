using FluentAssertions;
using Raiqub.Expressions.Tests.Examples;
using Raiqub.Expressions.Tests.Stubs;

namespace Raiqub.Expressions.Tests;

public class SpecificationTest
{
    [Theory]
    [InlineData("Jane Dee")]
    [InlineData("jane moor")]
    [InlineData(" john")]
    public void CreateShouldEvaluateCorrectly(string input)
    {
        var specification = Specification.Create<string>(
            x => x.StartsWith("john", StringComparison.InvariantCultureIgnoreCase));

        bool result = specification.IsSatisfiedBy(input);
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("Jane Dee")]
    [InlineData("jane moor")]
    [InlineData(" john")]
    public void AndShouldEvaluateFirstOnly(string input)
    {
        var specification = new StringBeginsWithJohnSpecification()
            .And(new ProhibitedSpecification());

        bool result = specification.IsSatisfiedBy(input);
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("John Dee")]
    [InlineData("john dee")]
    [InlineData("johndee")]
    public void AndShouldEvaluateBoth(string input)
    {
        var specification = new StringBeginsWithJohnSpecification()
            .And(new StringEndsWithDeeSpecification());

        bool result = specification.IsSatisfiedBy(input);
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("John Dee")]
    [InlineData("john dee")]
    [InlineData("johndee")]
    public void AndShouldReturnTrueForEmptyCollection(string input)
    {
        var specification1 = Specification.And<string>();
        var specification2 = Specification.And(Enumerable.Empty<Specification<string>>());

        bool result1 = specification1.IsSatisfiedBy(input);
        bool result2 = specification2.IsSatisfiedBy(input);

        result1.Should().BeTrue();
        result2.Should().BeTrue();
    }

    [Fact]
    public void AndShouldEvaluateCollectionCorrectly()
    {
        var specification = Specification.And(
            Specification.Create<string>(s => s.Length == 4),
            Specification.Create<string>(s => s[0] == 'j'),
            Specification.Create<string>(s => s[1] == 'o'),
            Specification.Create<string>(s => s[2] == 'h'),
            Specification.Create<string>(s => s[3] == 'n'));

        bool result1 = specification.IsSatisfiedBy("john");
        bool result2 = specification.IsSatisfiedBy("joh");
        bool result3 = specification.IsSatisfiedBy("johm");

        result1.Should().BeTrue();
        result2.Should().BeFalse();
        result3.Should().BeFalse();
    }

    [Theory]
    [InlineData("John Dee")]
    [InlineData("john moor")]
    [InlineData("john")]
    public void OrShouldEvaluateFirstOnly(string input)
    {
        var specification = new StringBeginsWithJohnSpecification()
            .Or(new ProhibitedSpecification());

        bool result = specification.IsSatisfiedBy(input);
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("Jane Dee")]
    [InlineData("jane dee")]
    [InlineData("dee")]
    public void OrShouldEvaluateBoth(string input)
    {
        var specification = new StringBeginsWithJohnSpecification()
            .Or(new StringEndsWithDeeSpecification());

        bool result = specification.IsSatisfiedBy(input);
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("John Dee")]
    [InlineData("john dee")]
    [InlineData("johndee")]
    public void OrShouldReturnTrueForEmptyCollection(string input)
    {
        var specification1 = Specification.Or<string>();
        var specification2 = Specification.Or(Enumerable.Empty<Specification<string>>());

        bool result1 = specification1.IsSatisfiedBy(input);
        bool result2 = specification2.IsSatisfiedBy(input);

        result1.Should().BeTrue();
        result2.Should().BeTrue();
    }

    [Fact]
    public void OrShouldEvaluateCollectionCorrectly()
    {
        var specification = Specification.Or(
            Specification.Create<string>(s => s.Length == 4),
            Specification.Create<string>(s => s[0] == 'j'),
            Specification.Create<string>(s => s[1] == 'o'),
            Specification.Create<string>(s => s[2] == 'h'),
            Specification.Create<string>(s => s[3] == 'n'));

        bool result1 = specification.IsSatisfiedBy("john");
        bool result2 = specification.IsSatisfiedBy("xuxu");
        bool result3 = specification.IsSatisfiedBy("lanne");
        bool result4 = specification.IsSatisfiedBy("lame word");

        result1.Should().BeTrue();
        result2.Should().BeTrue();
        result3.Should().BeTrue();
        result4.Should().BeFalse();
    }

    [Theory]
    [InlineData("John Dee")]
    [InlineData("john moor")]
    [InlineData("john")]
    public void NotShouldNegateExpression(string input)
    {
        var specification = new StringBeginsWithJohnSpecification()
            .Not();

        bool result = specification.IsSatisfiedBy(input);
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("John Dee")]
    [InlineData("john moor")]
    [InlineData("john")]
    public void NotShouldNegateTwiceExpression(string input)
    {
        var specification = new StringBeginsWithJohnSpecification()
            .Not()
            .Not();

        bool result = specification.IsSatisfiedBy(input);
        result.Should().BeTrue();
    }
}
