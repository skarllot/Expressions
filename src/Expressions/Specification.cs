using System.Linq.Expressions;

namespace Raiqub.Expressions;

public static class Specification
{
    public static Specification<T> Create<T>(Expression<Func<T, bool>> expression)
    {
        return new AnonymousSpecification<T>(expression);
    }

    public static Specification<T> And<T>(this Specification<T> left, Specification<T> right)
    {
        return new AnonymousSpecification<T>(left.ToExpression().And(right.ToExpression()));
    }

    public static Specification<T> And<T>(IEnumerable<Specification<T>> specifications)
    {
        return new AnonymousSpecification<T>(specifications.Select(static s => s.ToExpression()).And());
    }

    public static Specification<T> And<T>(params Specification<T>[] specifications)
    {
        return And((IEnumerable<Specification<T>>)specifications);
    }

    public static Specification<T> Not<T>(this Specification<T> specification)
    {
        return new AnonymousSpecification<T>(specification.ToExpression().Not());
    }

    public static Specification<T> Or<T>(this Specification<T> left, Specification<T> right)
    {
        return new AnonymousSpecification<T>(left.ToExpression().Or(right.ToExpression()));
    }

    public static Specification<T> Or<T>(IEnumerable<Specification<T>> specifications)
    {
        return new AnonymousSpecification<T>(specifications.Select(static s => s.ToExpression()).Or());
    }

    public static Specification<T> Or<T>(params Specification<T>[] specifications)
    {
        return Or((IEnumerable<Specification<T>>)specifications);
    }
}
