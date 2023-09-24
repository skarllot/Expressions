using System.Linq.Expressions;

namespace Raiqub.Expressions.Internal;

internal static class ExpressionTreeExtensions
{
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        var rightParam = right.Parameters.Single();
        var leftParam = left.Parameters.Single();

        var newRight = new ReplaceParameterExpressionVisitor(rightParam, leftParam)
            .VisitAndConvert(right.Body, nameof(Expression.AndAlso));

        var andExpression = Expression.AndAlso(left.Body, newRight);
        return Expression.Lambda<Func<T, bool>>(andExpression, leftParam);
    }

    public static Expression<Func<T, bool>> And<T>(this IEnumerable<Expression<Func<T, bool>>> expressions)
    {
        return expressions.Aggregate(
            AllSpecification<T>.s_expression,
            static (x, y) => ReferenceEquals(x, AllSpecification<T>.s_expression) ? y : x.And(y));
    }

    public static Expression<Func<TDerived, bool>> CastDown<TParent, TDerived>(
        this Expression<Func<TParent, bool>> expression)
        where TDerived : class, TParent
    {
        var originalParam = expression.Parameters.Single();
        var newParam = Expression.Parameter(typeof(TDerived), originalParam.Name);

        Expression newBody = new ReplaceParameterExpressionVisitor(originalParam, newParam)
            .VisitAndConvert(expression.Body, expression.Name);

        return Expression.Lambda<Func<TDerived, bool>>(newBody, newParam);
    }

    public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
    {
        var leftParam = expression.Parameters.Single();

        var notExpression = Expression.Not(expression.Body);
        return Expression.Lambda<Func<T, bool>>(notExpression, leftParam);
    }

    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        var rightParam = right.Parameters.Single();
        var leftParam = left.Parameters.Single();

        var newRight = new ReplaceParameterExpressionVisitor(rightParam, leftParam)
            .VisitAndConvert(right.Body, nameof(Expression.OrElse));

        var andExpression = Expression.OrElse(left.Body, newRight);
        return Expression.Lambda<Func<T, bool>>(andExpression, leftParam);
    }

    public static Expression<Func<T, bool>> Or<T>(this IEnumerable<Expression<Func<T, bool>>> expressions)
    {
        return expressions.Aggregate(
            AllSpecification<T>.s_expression,
            static (x, y) => ReferenceEquals(x, AllSpecification<T>.s_expression) ? y : x.Or(y));
    }
}
