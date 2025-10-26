using System.Linq.Expressions;
using AwesomeAssertions;
using Raiqub.Expressions.Tests.Stubs;

namespace Raiqub.Expressions.Tests;

public class SpecificationProtectedMethodsTest
{
    [Fact]
    public void And_WithTwoExpressions_ShouldCombineWithLogicalAnd()
    {
        Expression<Func<int, bool>> left = x => x > 5;
        Expression<Func<int, bool>> right = x => x < 10;

        var result = SpecificationHelper<int>.And(left, right);
        var compiled = result.Compile();

        compiled(7).Should().BeTrue();
        compiled(3).Should().BeFalse();
        compiled(12).Should().BeFalse();
    }

    [Fact]
    public void And_WithTwoExpressions_ShouldProduceCorrectExpressionString()
    {
        Expression<Func<string, bool>> left = s => s.Length > 2;
        Expression<Func<string, bool>> right = s => s.StartsWith('a');

        var result = SpecificationHelper<string>.And(left, right);

        result.ToString().Should().Contain("AndAlso");
    }

    [Fact]
    public void And_WithThreeExpressions_ShouldCombineWithLogicalAnd()
    {
        Expression<Func<int, bool>> first = x => x > 5;
        Expression<Func<int, bool>> second = x => x < 10;
        Expression<Func<int, bool>> third = x => x % 2 == 0;

        var result = SpecificationHelper<int>.And(first, second, third);
        var compiled = result.Compile();

        compiled(6).Should().BeTrue();
        compiled(8).Should().BeTrue();
        compiled(7).Should().BeFalse(); // odd
        compiled(3).Should().BeFalse(); // not > 5
        compiled(12).Should().BeFalse(); // not < 10
    }

    [Fact]
    public void And_WithParamsArray_ShouldCombineAllWithLogicalAnd()
    {
        Expression<Func<string, bool>>[] expressions =
        [
            s => s.Length == 4,
            s => s[0] == 't',
            s => s[1] == 'e',
            s => s[2] == 's',
            s => s[3] == 't',
        ];

        var result = SpecificationHelper<string>.And(expressions);
        var compiled = result.Compile();

        compiled("test").Should().BeTrue();
        compiled("best").Should().BeFalse(); // wrong first char
        compiled("tes").Should().BeFalse(); // wrong length
    }

    [Fact]
    public void And_WithEmptyParamsArray_ShouldReturnTrueExpression()
    {
        var result = SpecificationHelper<int>.And();
        var compiled = result.Compile();

        compiled(0).Should().BeTrue();
        compiled(100).Should().BeTrue();
        compiled(-50).Should().BeTrue();
    }

    [Fact]
    public void And_WithSingleExpressionInParamsArray_ShouldReturnSameExpression()
    {
        Expression<Func<int, bool>>[] expressions = [x => x > 5];

        var result = SpecificationHelper<int>.And(expressions);
        var compiled = result.Compile();

        result.Should().BeSameAs(expressions[0]);
        compiled(10).Should().BeTrue();
        compiled(3).Should().BeFalse();
    }

    [Fact]
    public void And_WithEnumerable_ShouldCombineAllWithLogicalAnd()
    {
        IEnumerable<Expression<Func<int, bool>>> expressions = new List<Expression<Func<int, bool>>>
        {
            x => x > 0,
            x => x < 100,
            x => x % 5 == 0,
        };

        var result = SpecificationHelper<int>.And(expressions);
        var compiled = result.Compile();

        compiled(10).Should().BeTrue();
        compiled(50).Should().BeTrue();
        compiled(3).Should().BeFalse(); // not divisible by 5
        compiled(-5).Should().BeFalse(); // not > 0
        compiled(105).Should().BeFalse(); // not < 100
    }

    [Fact]
    public void And_WithEnumerableEmpty_ShouldReturnTrueExpression()
    {
        var expressions = Enumerable.Empty<Expression<Func<int, bool>>>();

        var result = SpecificationHelper<int>.And(expressions);
        var compiled = result.Compile();

        compiled(42).Should().BeTrue();
    }

    [Fact]
    public void Not_ShouldNegateExpression()
    {
        Expression<Func<int, bool>> expression = x => x > 5;

        var result = SpecificationHelper<int>.Not(expression);
        var compiled = result.Compile();

        compiled(10).Should().BeFalse();
        compiled(3).Should().BeTrue();
        compiled(5).Should().BeTrue();
    }

    [Fact]
    public void Not_ShouldProduceCorrectExpressionString()
    {
        Expression<Func<bool, bool>> expression = b => b;

        var result = SpecificationHelper<bool>.Not(expression);

        result.ToString().Should().Contain("Not");
    }

    [Fact]
    public void Not_WithComplexExpression_ShouldNegate()
    {
        Expression<Func<string, bool>> expression = s => s.Length > 3 && s.StartsWith('a');

        var result = SpecificationHelper<string>.Not(expression);
        var compiled = result.Compile();

        compiled("abc").Should().BeTrue(); // length not > 3
        compiled("test").Should().BeTrue(); // doesn't start with 'a'
        compiled("apple").Should().BeFalse(); // satisfies original
    }

    [Fact]
    public void Or_WithTwoExpressions_ShouldCombineWithLogicalOr()
    {
        Expression<Func<int, bool>> left = x => x < 5;
        Expression<Func<int, bool>> right = x => x > 10;

        var result = SpecificationHelper<int>.Or(left, right);
        var compiled = result.Compile();

        compiled(3).Should().BeTrue();
        compiled(12).Should().BeTrue();
        compiled(7).Should().BeFalse();
    }

    [Fact]
    public void Or_WithTwoExpressions_ShouldProduceCorrectExpressionString()
    {
        Expression<Func<string, bool>> left = s => s.Length > 10;
        Expression<Func<string, bool>> right = s => s.StartsWith('x');

        var result = SpecificationHelper<string>.Or(left, right);

        result.ToString().Should().Contain("OrElse");
    }

    [Fact]
    public void Or_WithThreeExpressions_ShouldCombineWithLogicalOr()
    {
        Expression<Func<int, bool>> first = x => x < 0;
        Expression<Func<int, bool>> second = x => x > 100;
        Expression<Func<int, bool>> third = x => x == 50;

        var result = SpecificationHelper<int>.Or(first, second, third);
        var compiled = result.Compile();

        compiled(-5).Should().BeTrue();
        compiled(150).Should().BeTrue();
        compiled(50).Should().BeTrue();
        compiled(25).Should().BeFalse();
    }

    [Fact]
    public void Or_WithParamsArray_ShouldCombineAllWithLogicalOr()
    {
        Expression<Func<string, bool>>[] expressions =
        [
            s => s == "one",
            s => s == "two",
            s => s == "three",
            s => s == "four",
        ];

        var result = SpecificationHelper<string>.Or(expressions);
        var compiled = result.Compile();

        compiled("one").Should().BeTrue();
        compiled("two").Should().BeTrue();
        compiled("three").Should().BeTrue();
        compiled("four").Should().BeTrue();
        compiled("five").Should().BeFalse();
    }

    [Fact]
    public void Or_WithEmptyParamsArray_ShouldReturnTrueExpression()
    {
        var result = SpecificationHelper<int>.Or(Array.Empty<Expression<Func<int, bool>>>());
        var compiled = result.Compile();

        compiled(0).Should().BeTrue();
        compiled(100).Should().BeTrue();
        compiled(-50).Should().BeTrue();
    }

    [Fact]
    public void Or_WithSingleExpressionInParamsArray_ShouldReturnSameExpression()
    {
        Expression<Func<int, bool>>[] expressions = [x => x > 5];

        var result = SpecificationHelper<int>.Or(expressions);
        var compiled = result.Compile();

        compiled(10).Should().BeTrue();
        compiled(3).Should().BeFalse();
    }

    [Fact]
    public void Or_WithEnumerable_ShouldCombineAllWithLogicalOr()
    {
        IEnumerable<Expression<Func<int, bool>>> expressions = new List<Expression<Func<int, bool>>>
        {
            x => x == 10,
            x => x == 20,
            x => x == 30,
        };

        var result = SpecificationHelper<int>.Or(expressions);
        var compiled = result.Compile();

        compiled(10).Should().BeTrue();
        compiled(20).Should().BeTrue();
        compiled(30).Should().BeTrue();
        compiled(15).Should().BeFalse();
    }

    [Fact]
    public void Or_WithEnumerableEmpty_ShouldReturnTrueExpression()
    {
        var expressions = Enumerable.Empty<Expression<Func<int, bool>>>();

        var result = SpecificationHelper<int>.Or(expressions);
        var compiled = result.Compile();

        compiled(42).Should().BeTrue();
    }

    [Fact]
    public void And_Or_Not_Combined_ShouldWorkCorrectly()
    {
        Expression<Func<int, bool>> isPositive = x => x > 0;
        Expression<Func<int, bool>> isEven = x => x % 2 == 0;
        Expression<Func<int, bool>> isSmall = x => x < 10;

        // (positive AND even) OR NOT small
        // Which is: (x > 0 && x % 2 == 0) || (x >= 10)
        var positiveAndEven = SpecificationHelper<int>.And(isPositive, isEven);
        var notSmall = SpecificationHelper<int>.Not(isSmall);
        var result = SpecificationHelper<int>.Or(positiveAndEven, notSmall);
        var compiled = result.Compile();

        compiled(2).Should().BeTrue(); // positive and even (satisfies first part)
        compiled(4).Should().BeTrue(); // positive and even (satisfies first part)
        compiled(20).Should().BeTrue(); // positive, even, and not small (satisfies both parts)
        compiled(15).Should().BeTrue(); // not small (satisfies second part)
        compiled(11).Should().BeTrue(); // not small (satisfies second part)
        compiled(100).Should().BeTrue(); // not small (satisfies second part)
        compiled(3).Should().BeFalse(); // positive but odd, and small
        compiled(5).Should().BeFalse(); // positive but odd, and small
        compiled(-5).Should().BeFalse(); // negative, small
    }
}
