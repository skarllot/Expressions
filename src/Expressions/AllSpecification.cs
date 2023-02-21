using System.Linq.Expressions;

namespace Raiqub.Expressions;

internal sealed class AllSpecification<T> : Specification<T>
{
    public static readonly Specification<T> Instance = new AllSpecification<T>();

    public override Expression<Func<T, bool>> ToExpression() => static _ => true;
}
