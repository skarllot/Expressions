using System.Linq.Expressions;

namespace Raiqub.Expressions.Tests.Stubs;

public class OneTimeSpecification<T> : Specification<T>
{
    private readonly Expression<Func<T, bool>> _expression;
    private int _count;

    public OneTimeSpecification(Expression<Func<T, bool>> expression)
    {
        _expression = expression;
    }

    public override Expression<Func<T, bool>> ToExpression()
    {
        return _count++ == 0
            ? _expression
            : throw new InvalidOperationException();
    }
}
