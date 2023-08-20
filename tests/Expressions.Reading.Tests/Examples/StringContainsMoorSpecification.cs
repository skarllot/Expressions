using System.Linq.Expressions;

namespace Raiqub.Expressions.Reading.Tests.Examples;

internal sealed class StringContainsMoorSpecification : Specification<string>
{
    public override Expression<Func<string, bool>> ToExpression()
    {
#if NET5_0_OR_GREATER
        return input => input.Contains("moor", StringComparison.InvariantCultureIgnoreCase);
#else
        return input => input.IndexOf("moor", StringComparison.InvariantCultureIgnoreCase) >= 0;
#endif
    }
}
