using System.Linq.Expressions;

namespace Raiqub.Expressions;

public abstract class Specification<T>
{
    private Func<T, bool>? _predicate;

    public static Specification<T> All => AllSpecification<T>.Instance;

    public static Specification<T> operator &(Specification<T> left, Specification<T> right) => left.And(right);
    public static Specification<T> operator |(Specification<T> left, Specification<T> right) => left.Or(right);
    public static Specification<T> operator !(Specification<T> specification) => specification.Not();

    public abstract Expression<Func<T, bool>> ToExpression();

    public bool IsSatisfiedBy(T entity)
    {
        _predicate ??= ToExpression().Compile();
        return _predicate(entity);
    }

    public override string ToString() => ToExpression().ToString();
}
