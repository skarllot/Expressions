using System.Linq.Expressions;

namespace Raiqub.Expressions.Reading.Tests.Examples;

internal sealed class StringBeginsWithJohnSpecification : Specification<string>
{
    public override Expression<Func<string, bool>> ToExpression()
    {
        return input => input.StartsWith("john", StringComparison.InvariantCultureIgnoreCase);
    }
}
