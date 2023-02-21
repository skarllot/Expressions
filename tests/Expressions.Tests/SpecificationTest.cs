using System.Linq.Expressions;
using FluentAssertions;
using Xunit.Sdk;

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

    private sealed class StringBeginsWithJohnSpecification : Specification<string>
    {
        public override Expression<Func<string, bool>> ToExpression()
        {
            return input => input.StartsWith("john", StringComparison.InvariantCultureIgnoreCase);
        }
    }

    private sealed class StringEndsWithDeeSpecification : Specification<string>
    {
        public override Expression<Func<string, bool>> ToExpression()
        {
            return input => input.EndsWith("dee", StringComparison.InvariantCultureIgnoreCase);
        }
    }

    private sealed class ProhibitedSpecification : Specification<string>
    {
        public override Expression<Func<string, bool>> ToExpression()
        {
            return input => InternalExpression();
        }

        private static bool InternalExpression()
        {
            throw new XunitException("The expression should not be evaluated");
        }
    }
}
