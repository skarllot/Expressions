using System.Linq.Expressions;

namespace Raiqub.Expressions.Internal;

internal sealed class AnonymousSpecification<T> : Specification<T>
{
    private readonly Expression<Func<T, bool>> _expression;

    public AnonymousSpecification(Expression<Func<T, bool>> expression) => _expression = expression;

    public override Expression<Func<T, bool>> ToExpression() => _expression;
}
