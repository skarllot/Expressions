using System.Linq.Expressions;
using Raiqub.Expressions.Internal;

namespace Raiqub.Expressions;

/// <summary>
/// Provides initialization and combination methods for instances of the <see cref="Specification{T}"/> class.
/// </summary>
public static class Specification
{
    /// <summary>Returns a specification that allows any instance of <typeparamref name="T"/>.</summary>
    /// <typeparam name="T">The type of the object that this specification can be applied to.</typeparam>
    /// <returns>A no-op specification.</returns>
    public static Specification<T> All<T>()
    {
        return AllSpecification<T>.Instance;
    }

    /// <summary>Creates a new specification that contains the specified expression.</summary>
    /// <param name="expression">A lambda expression defining the business rule.</param>
    /// <typeparam name="T">The type of the object that this specification can be applied to.</typeparam>
    /// <returns>A new instance of <see cref="Specification{T}"/> representing the specified business rule.</returns>
    public static Specification<T> Create<T>(Expression<Func<T, bool>> expression)
    {
        return new AnonymousSpecification<T>(expression);
    }

    /// <summary>Combines the two specifications using the conditional logical AND operator.</summary>
    /// <param name="left">The left operand of AND operator.</param>
    /// <param name="right">The right operand of AND operator.</param>
    /// <typeparam name="T">The type of the object that this specification can be applied to.</typeparam>
    /// <returns>A new specification that represents the conjunction of two specifications (both must be satisfied).</returns>
    public static Specification<T> And<T>(this Specification<T> left, Specification<T> right)
    {
        if (left.IsTrueExpression())
            return right;
        if (right.IsTrueExpression())
            return left;
        return new AnonymousSpecification<T>(left.ToExpression().And(right.ToExpression()));
    }

    /// <summary>Combines multiple specifications using the conditional logical AND operator.</summary>
    /// <param name="specifications">The specifications to combine sequentially.</param>
    /// <typeparam name="T">The type of the object that this specification can be applied to.</typeparam>
    /// <returns>A new specification that represents the conjunction of all specifications (all must be satisfied).</returns>
    public static Specification<T> And<T>(IEnumerable<Specification<T>> specifications)
    {
        return ReferenceEquals(specifications, Enumerable.Empty<Specification<T>>())
            ? AllSpecification<T>.Instance
            : new AnonymousSpecification<T>(specifications.Select(static s => s.ToExpression()).And());
    }

    /// <summary>Combines multiple specifications using the conditional logical AND operator.</summary>
    /// <param name="specifications">The specifications to combine sequentially.</param>
    /// <typeparam name="T">The type of the object that this specification can be applied to.</typeparam>
    /// <returns>A new specification that represents the conjunction of all specifications (all must be satisfied).</returns>
    public static Specification<T> And<T>(params Specification<T>[] specifications)
    {
        return And((IEnumerable<Specification<T>>)specifications);
    }

    /// <summary>Negate a specification using the logical negation operator.</summary>
    /// <param name="specification">The specification to negate.</param>
    /// <typeparam name="T">The type of the object that this specification can be applied to.</typeparam>
    /// <returns>A new specification that represents the negation of a specification (the opposite of it must be satisfied).</returns>
    public static Specification<T> Not<T>(this Specification<T> specification)
    {
        return new AnonymousSpecification<T>(specification.ToExpression().Not());
    }

    /// <summary>Combines the two specifications using the conditional logical OR operator.</summary>
    /// <param name="left">The left operand of OR operator.</param>
    /// <param name="right">The right operand of OR operator.</param>
    /// <typeparam name="T">The type of the object that this specification can be applied to.</typeparam>
    /// <returns>A new specification that represents the disjunction of two specifications (at least one of them must be satisfied).</returns>
    public static Specification<T> Or<T>(this Specification<T> left, Specification<T> right)
    {
        return left.IsTrueExpression() || right.IsTrueExpression()
            ? AllSpecification<T>.Instance
            : new AnonymousSpecification<T>(left.ToExpression().Or(right.ToExpression()));
    }

    /// <summary>Combines multiple specifications using the conditional logical OR operator.</summary>
    /// <param name="specifications">The specifications to combine sequentially.</param>
    /// <typeparam name="T">The type of the object that this specification can be applied to.</typeparam>
    /// <returns>A new specification that represents the disjunction of all specifications (at least one of them must be satisfied).</returns>
    public static Specification<T> Or<T>(IEnumerable<Specification<T>> specifications)
    {
        return ReferenceEquals(specifications, Enumerable.Empty<Specification<T>>())
            ? AllSpecification<T>.Instance
            : new AnonymousSpecification<T>(specifications.Select(static s => s.ToExpression()).Or());
    }

    /// <summary>Combines multiple specifications using the conditional logical OR operator.</summary>
    /// <param name="specifications">The specifications to combine sequentially.</param>
    /// <typeparam name="T">The type of the object that this specification can be applied to.</typeparam>
    /// <returns>A new specification that represents the disjunction of all specifications (at least one of them must be satisfied).</returns>
    public static Specification<T> Or<T>(params Specification<T>[] specifications)
    {
        return Or((IEnumerable<Specification<T>>)specifications);
    }
}
