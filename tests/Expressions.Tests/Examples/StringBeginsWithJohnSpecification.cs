using System.Linq.Expressions;

namespace Raiqub.Expressions.Tests.Examples;

internal sealed class StringBeginsWithJohnSpecification : Specification<string>
{
    public override Expression<Func<string, bool>> ToExpression()
    {
        return input => input.StartsWith("john", StringComparison.InvariantCultureIgnoreCase);
    }
}
