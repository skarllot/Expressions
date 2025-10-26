using System.Linq.Expressions;

namespace Raiqub.Expressions.Tests.Stubs;

/// <summary>Helper class to expose protected static methods from Specification for testing.</summary>
internal sealed class SpecificationHelper<T>(Expression<Func<T, bool>> expression) : Specification<T>
{
    public override Expression<Func<T, bool>> ToExpression() => expression;

    public static new Expression<Func<T, bool>> And(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right) =>
        Specification<T>.And(left, right);

    public static new Expression<Func<T, bool>> And(
        Expression<Func<T, bool>> first,
        Expression<Func<T, bool>> second,
        Expression<Func<T, bool>> third
    ) => Specification<T>.And(first, second, third);

    public static new Expression<Func<T, bool>> And(params Expression<Func<T, bool>>[] expressions) =>
        Specification<T>.And(expressions);

    public static new Expression<Func<T, bool>> And(IEnumerable<Expression<Func<T, bool>>> expressions) =>
        Specification<T>.And(expressions);

    public static new Expression<Func<T, bool>> Not(Expression<Func<T, bool>> expression) =>
        Specification<T>.Not(expression);

    public static new Expression<Func<T, bool>> Or(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right) =>
        Specification<T>.Or(left, right);

    public static new Expression<Func<T, bool>> Or(
        Expression<Func<T, bool>> first,
        Expression<Func<T, bool>> second,
        Expression<Func<T, bool>> third
    ) => Specification<T>.Or(first, second, third);

    public static new Expression<Func<T, bool>> Or(params Expression<Func<T, bool>>[] expressions) =>
        Specification<T>.Or(expressions);

    public static new Expression<Func<T, bool>> Or(IEnumerable<Expression<Func<T, bool>>> expressions) =>
        Specification<T>.Or(expressions);
}
