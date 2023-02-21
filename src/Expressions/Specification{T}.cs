using System.Linq.Expressions;

namespace Raiqub.Expressions;

public abstract class Specification<T>
{
    private Func<T, bool>? _predicate;

    public static Specification<T> All => AllSpecification<T>.Instance;

    public abstract Expression<Func<T, bool>> ToExpression();

    public bool IsSatisfiedBy(T entity)
    {
        _predicate ??= ToExpression().Compile();
        return _predicate(entity);
    }

    public override string ToString() => ToExpression().ToString();
}
