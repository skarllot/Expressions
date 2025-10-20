using System.Linq.Expressions;
using AwesomeAssertions;
using Raiqub.Expressions.Internal;

namespace Raiqub.Expressions.Tests.Internal;

public class ExpressionTreeExtensionsTest
{
    [Fact]
    public void AndShouldIgnoreAllSpecifications()
    {
        Expression<Func<string, bool>> trueExpr = s => true;
        Expression<Func<string, bool>> isJohnExpr = s => s == "john";

        var spec1 = trueExpr.And(isJohnExpr);
        var spec2 = isJohnExpr.And(trueExpr);

        spec1.Should().BeSameAs(isJohnExpr);
        spec2.Should().BeSameAs(isJohnExpr);
    }

    [Fact]
    public void OrShouldOptimizeAllSpecifications()
    {
        Expression<Func<string, bool>> customSpec = s => true;
        Expression<Func<string, bool>> isJohnSpec = s => s == "john";

        var spec1 = customSpec.Or(isJohnSpec);
        var spec2 = isJohnSpec.Or(customSpec);

        spec1.Should().BeSameAs(AllSpecification<string>.s_expression);
        spec2.Should().BeSameAs(AllSpecification<string>.s_expression);
    }
}
