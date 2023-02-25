using System.Linq.Expressions;

namespace Raiqub.Expressions.Tests.Examples;

internal sealed class StringEndsWithDeeSpecification : Specification<string>
{
    public override Expression<Func<string, bool>> ToExpression()
    {
        return input => input.EndsWith("dee", StringComparison.InvariantCultureIgnoreCase);
    }
}
