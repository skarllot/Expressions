using System.Linq.Expressions;
using Raiqub.Expressions.Internal;

namespace Raiqub.Expressions;

/// <summary>Represents a combinable and reusable business rule.</summary>
/// <typeparam name="T">The type of the object that this specification can be applied to.</typeparam>
public abstract class Specification<T>
{
    private Func<T, bool>? _predicate;

    /// <summary>
    /// Returns a new specification that represents the conjunction of two specifications (both must be satisfied).
    /// </summary>
    public static Specification<T> operator &(Specification<T> left, Specification<T> right) => left.And(right);

    /// <summary>
    /// Returns a new specification that represents the disjunction of two specifications (at least one of them must be satisfied).
    /// </summary>
    public static Specification<T> operator |(Specification<T> left, Specification<T> right) => left.Or(right);

    /// <summary>
    /// Returns a new specification that represents the negation of a specification (the opposite of it must be satisfied).
    /// </summary>
    public static Specification<T> operator !(Specification<T> specification) => specification.Not();

    /// <summary>
    /// Returns an expression that represents the specification as a lambda expression.
    /// </summary>
    public abstract Expression<Func<T, bool>> ToExpression();

    /// <summary>
    /// Returns a new specification that represents the same criteria but applies to objects
    /// of type <typeparamref name="TDerived"/>, which is a subclass of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="TDerived">The type of the derived object(s) that this specification can be applied to.</typeparam>
    public Specification<TDerived> CastDown<TDerived>()
        where TDerived : class, T
    {
        return new AnonymousSpecification<TDerived>(ToExpression().CastDown<T, TDerived>());
    }

    /// <summary>
    /// Evaluates the business rule represented by this instance against a given object and returns a boolean value
    /// that indicates whether the object satisfies the specification.
    /// </summary>
    /// <param name="entity">The entity to check.</param>
    /// <returns>True if the entity satisfies the specification, otherwise false.</returns>
    public bool IsSatisfiedBy(T entity)
    {
        _predicate ??= ToExpression().Compile();
        return _predicate(entity);
    }

    /// <summary>Returns a string representation of the lambda expression.</summary>
    public override string ToString() => ToExpression().ToString();

    internal bool IsTrueExpression() => ToExpression().IsTrueConstant();

    /// <summary>Combines two expressions with a logical AND operation.</summary>
    /// <param name="left">The left expression.</param>
    /// <param name="right">The right expression.</param>
    /// <returns>A new expression representing the logical AND of both expressions.</returns>
    protected static Expression<Func<T, bool>> And(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right) =>
        left.And(right);

    /// <summary>Combines three expressions with a logical AND operation.</summary>
    /// <param name="first">The first expression.</param>
    /// <param name="second">The second expression.</param>
    /// <param name="third">The third expression.</param>
    /// <returns>A new expression representing the logical AND of all three expressions.</returns>
    protected static Expression<Func<T, bool>> And(
        Expression<Func<T, bool>> first,
        Expression<Func<T, bool>> second,
        Expression<Func<T, bool>> third
    ) => ExpressionTreeExtensions.And([first, second, third]);

    /// <summary>Combines multiple expressions with a logical AND operation.</summary>
    /// <param name="expressions">The array of expressions to combine.</param>
    /// <returns>A new expression representing the logical AND of all expressions.</returns>
    protected static Expression<Func<T, bool>> And(params Expression<Func<T, bool>>[] expressions) =>
        ExpressionTreeExtensions.And([.. expressions]);

    /// <summary>Combines multiple expressions with a logical AND operation.</summary>
    /// <param name="expressions">The collection of expressions to combine.</param>
    /// <returns>A new expression representing the logical AND of all expressions.</returns>
    protected static Expression<Func<T, bool>> And(IEnumerable<Expression<Func<T, bool>>> expressions) =>
        expressions.And();

    /// <summary>Negates an expression with a logical NOT operation.</summary>
    /// <param name="expression">The expression to negate.</param>
    /// <returns>A new expression representing the logical NOT of the expression.</returns>
    protected static Expression<Func<T, bool>> Not(Expression<Func<T, bool>> expression) => expression.Not();

    /// <summary>Combines two expressions with a logical OR operation.</summary>
    /// <param name="left">The left expression.</param>
    /// <param name="right">The right expression.</param>
    /// <returns>A new expression representing the logical OR of both expressions.</returns>
    protected static Expression<Func<T, bool>> Or(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right) =>
        left.Or(right);

    /// <summary>Combines three expressions with a logical OR operation.</summary>
    /// <param name="first">The first expression.</param>
    /// <param name="second">The second expression.</param>
    /// <param name="third">The third expression.</param>
    /// <returns>A new expression representing the logical OR of all three expressions.</returns>
    protected static Expression<Func<T, bool>> Or(
        Expression<Func<T, bool>> first,
        Expression<Func<T, bool>> second,
        Expression<Func<T, bool>> third
    ) => ExpressionTreeExtensions.Or([first, second, third]);

    /// <summary>Combines multiple expressions with a logical OR operation.</summary>
    /// <param name="expressions">The array of expressions to combine.</param>
    /// <returns>A new expression representing the logical OR of all expressions.</returns>
    protected static Expression<Func<T, bool>> Or(params Expression<Func<T, bool>>[] expressions) =>
        ExpressionTreeExtensions.Or([.. expressions]);

    /// <summary>Combines multiple expressions with a logical OR operation.</summary>
    /// <param name="expressions">The collection of expressions to combine.</param>
    /// <returns>A new expression representing the logical OR of all expressions.</returns>
    protected static Expression<Func<T, bool>> Or(IEnumerable<Expression<Func<T, bool>>> expressions) =>
        expressions.Or();
}
