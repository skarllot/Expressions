using System.Linq.Expressions;
using Raiqub.Expressions.Common;

namespace Raiqub.Expressions.Internal;

internal static class ExpressionTreeExtensions
{
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        if (IsTrueConstant(left))
            return right;
        if (IsTrueConstant(right))
            return left;

        var rightParam = right.Parameters[0];
        var leftParam = left.Parameters[0];

        var newRight = new ReplaceParameterExpressionVisitor(rightParam, leftParam)
            .VisitAndConvert(right.Body, nameof(Expression.AndAlso));

        var andExpression = Expression.AndAlso(left.Body, newRight);
        return Expression.Lambda<Func<T, bool>>(andExpression, left.Parameters);
    }

    public static Expression<Func<T, bool>> And<T>(this ReadOnlySpan<Expression<Func<T, bool>>> expressions)
    {
        return expressions.AggregateOrDefault(static (x, y) => x.And(y), AllSpecification<T>.s_expression);
    }

    public static Expression<Func<T, bool>> And<T>(this IEnumerable<Expression<Func<T, bool>>> expressions)
    {
        return expressions.AggregateOrDefault(static (x, y) => x.And(y), AllSpecification<T>.s_expression);
    }

    public static Expression<Func<TDerived, bool>> CastDown<TParent, TDerived>(
        this Expression<Func<TParent, bool>> expression)
        where TDerived : class, TParent
    {
        var originalParam = expression.Parameters[0];
        var newParam = Expression.Parameter(typeof(TDerived), originalParam.Name);

        Expression newBody = new ReplaceParameterExpressionVisitor(originalParam, newParam)
            .VisitAndConvert(expression.Body, expression.Name);

        return Expression.Lambda<Func<TDerived, bool>>(newBody, newParam);
    }

    public static bool IsTrueConstant<T>(this Expression<Func<T, bool>> expression)
    {
        return expression.Body is ConstantExpression { Value: true };
    }

    public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
    {
        var notExpression = Expression.Not(expression.Body);
        return Expression.Lambda<Func<T, bool>>(notExpression, expression.Parameters);
    }

    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        if (IsTrueConstant(left) || IsTrueConstant(right))
            return AllSpecification<T>.s_expression;

        var rightParam = right.Parameters[0];
        var leftParam = left.Parameters[0];

        var newRight = new ReplaceParameterExpressionVisitor(rightParam, leftParam)
            .VisitAndConvert(right.Body, nameof(Expression.OrElse));

        var andExpression = Expression.OrElse(left.Body, newRight);
        return Expression.Lambda<Func<T, bool>>(andExpression, left.Parameters);
    }

    public static Expression<Func<T, bool>> Or<T>(this ReadOnlySpan<Expression<Func<T, bool>>> expressions)
    {
        return expressions.AggregateOrDefault(static (x, y) => x.Or(y), AllSpecification<T>.s_expression);
    }

    public static Expression<Func<T, bool>> Or<T>(this IEnumerable<Expression<Func<T, bool>>> expressions)
    {
        return expressions.AggregateOrDefault(static (x, y) => x.Or(y), AllSpecification<T>.s_expression);
    }
}
