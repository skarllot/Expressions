using System.Linq.Expressions;

namespace Raiqub.Specification;

public static class Specification
{
    public static Specification<T> Create<T>(Expression<Func<T, bool>> expression)
    {
        return new AnonymousSpecification<T>(expression);
    }

    public static Specification<T> And<T>(this Specification<T> left, Specification<T> right)
    {
        var rightExpression = right.ToExpression();
        var leftExpression = left.ToExpression();
        var rightParam = rightExpression.Parameters.Single();
        var leftParam = leftExpression.Parameters.Single();

        var newRight = new ReplaceParameterExpressionVisitor(rightParam, leftParam)
            .VisitAndConvert(rightExpression.Body, nameof(Expression.AndAlso));

        var andExpression = Expression.AndAlso(leftExpression.Body, newRight);
        return new AnonymousSpecification<T>(Expression.Lambda<Func<T, bool>>(andExpression, leftParam));
    }

    public static Specification<T> And<T>(IEnumerable<Specification<T>> specifications)
    {
        return specifications.Aggregate(AllSpecification<T>.Instance, static (x, y) => x.And(y));
    }

    public static Specification<T> And<T>(params Specification<T>[] specifications)
    {
        return And((IEnumerable<Specification<T>>)specifications);
    }

    public static Specification<T> Not<T>(this Specification<T> specification)
    {
        var expression = specification.ToExpression();
        var leftParam = expression.Parameters.Single();

        var notExpression = Expression.Not(expression.Body);
        return new AnonymousSpecification<T>(Expression.Lambda<Func<T, bool>>(notExpression, leftParam));
    }

    public static Specification<T> Or<T>(this Specification<T> left, Specification<T> right)
    {
        var rightExpression = right.ToExpression();
        var leftExpression = left.ToExpression();
        var rightParam = rightExpression.Parameters.Single();
        var leftParam = leftExpression.Parameters.Single();

        var newRight = new ReplaceParameterExpressionVisitor(rightParam, leftParam)
            .VisitAndConvert(rightExpression.Body, nameof(Expression.OrElse));

        var andExpression = Expression.OrElse(leftExpression.Body, newRight);
        return new AnonymousSpecification<T>(Expression.Lambda<Func<T, bool>>(andExpression, leftParam));
    }

    public static Specification<T> Or<T>(IEnumerable<Specification<T>> specifications)
    {
        return specifications.Aggregate(AllSpecification<T>.Instance, static (x, y) => x.Or(y));
    }

    public static Specification<T> Or<T>(params Specification<T>[] specifications)
    {
        return Or((IEnumerable<Specification<T>>)specifications);
    }
}
