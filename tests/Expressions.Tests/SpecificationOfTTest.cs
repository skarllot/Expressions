using FluentAssertions;
using Raiqub.Expressions.Tests.Examples;
using Raiqub.Expressions.Tests.Stubs;

namespace Raiqub.Expressions.Tests;

public class SpecificationOfTTest
{
    [Fact]
    public void ToStringShouldReturnExpectedExpression()
    {
        var specification = Specification.Create<string>(x => x.Length > 0);

        string result = specification.ToString();
        result.Should().Be("x => (x.Length > 0)");
    }

    [Theory]
    [InlineData("Jane Dee")]
    [InlineData("jane moor")]
    [InlineData(" john")]
    public void AndShouldEvaluateFirstOnly(string input)
    {
        var specification = new StringBeginsWithJohnSpecification() &
                            new ProhibitedSpecification();

        bool result = specification.IsSatisfiedBy(input);
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("John Dee")]
    [InlineData("john dee")]
    [InlineData("johndee")]
    public void AndShouldEvaluateBoth(string input)
    {
        var specification = new StringBeginsWithJohnSpecification() &
                            new StringEndsWithDeeSpecification();

        bool result = specification.IsSatisfiedBy(input);
        result.Should().BeTrue();
    }

    [Fact]
    public void AndShouldEvaluateMultipleCorrectly()
    {
        var specification = Specification.Create<string>(s => s.Length == 4) &
                            Specification.Create<string>(s => s[0] == 'j') &
                            Specification.Create<string>(s => s[1] == 'o') &
                            Specification.Create<string>(s => s[2] == 'h') &
                            Specification.Create<string>(s => s[3] == 'n');

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
        var specification = new StringBeginsWithJohnSpecification() |
                            new ProhibitedSpecification();

        bool result = specification.IsSatisfiedBy(input);
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("Jane Dee")]
    [InlineData("jane dee")]
    [InlineData("dee")]
    public void OrShouldEvaluateBoth(string input)
    {
        var specification = new StringBeginsWithJohnSpecification() |
                            new StringEndsWithDeeSpecification();

        bool result = specification.IsSatisfiedBy(input);
        result.Should().BeTrue();
    }

    [Fact]
    public void OrShouldEvaluateCollectionCorrectly()
    {
        var specification = Specification.Create<string>(s => s.Length == 4) |
                            Specification.Create<string>(s => s[0] == 'j') |
                            Specification.Create<string>(s => s[1] == 'o') |
                            Specification.Create<string>(s => s[2] == 'h') |
                            Specification.Create<string>(s => s[3] == 'n');

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
        var specification = !new StringBeginsWithJohnSpecification();

        bool result = specification.IsSatisfiedBy(input);
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("John Dee")]
    [InlineData("john moor")]
    [InlineData("john")]
    public void NotShouldNegateTwiceExpression(string input)
    {
        var preSpecification = !new StringBeginsWithJohnSpecification();
        var specification = !preSpecification;

        bool result = specification.IsSatisfiedBy(input);
        result.Should().BeTrue();
    }

    [Fact]
    public void CastDownShouldWorkWithDerived()
    {
        var specification = new BlogPostContentSearchSpecification("john").CastDown<NewsPost>();
        var input1 = new NewsPost(1, "Test", "My name is john.", "general", "John");
        var input2 = new NewsPost(1, "Test", "My name is jane.", "general", "Jane");

        bool result1 = specification.IsSatisfiedBy(input1);
        bool result2 = specification.IsSatisfiedBy(input2);
        result1.Should().BeTrue();
        result2.Should().BeFalse();
    }
}
