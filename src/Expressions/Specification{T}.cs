using System.Linq.Expressions;

namespace Raiqub.Expressions;

/// <summary>
/// Represents a specification or a set of criteria that can be applied to an object or a collection of
/// objects of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the object(s) that this specification can be applied to.</typeparam>
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
    /// Evaluates the specification against a given object and returns a boolean value that indicates whether
    /// the object satisfies the specification.
    /// </summary>
    /// <param name="entity">The entity to check.</param>
    /// <returns>True if the entity satisfies the specification, otherwise false.</returns>
    public bool IsSatisfiedBy(T entity)
    {
        _predicate ??= ToExpression().Compile();
        return _predicate(entity);
    }

    /// <summary>
    /// Returns a string representation of the lambda expression.
    /// </summary>
    public override string ToString() => ToExpression().ToString();
}
