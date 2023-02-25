using System.Linq.Expressions;
using Xunit.Sdk;

namespace Raiqub.Expressions.Tests.Stubs;

internal sealed class ProhibitedSpecification : Specification<string>
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
