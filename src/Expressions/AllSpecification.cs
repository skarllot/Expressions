using System.Linq.Expressions;

namespace Raiqub.Expressions;

internal sealed class AllSpecification<T> : Specification<T>
{
    public static readonly Specification<T> Instance = new AllSpecification<T>();

    internal static readonly Expression<Func<T, bool>> s_expression = static _ => true;

    public override Expression<Func<T, bool>> ToExpression() => s_expression;
}
