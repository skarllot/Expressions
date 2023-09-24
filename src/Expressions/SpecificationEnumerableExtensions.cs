using System.Diagnostics.CodeAnalysis;

namespace Raiqub.Expressions;

/// <summary>
/// Provides extensions for using <see cref="Specification{T}"/> with <see cref="IEnumerable{T}"/>
/// and <see cref="IQueryable{T}"/> instances.
/// </summary>
public static class SpecificationEnumerableExtensions
{
    /// <summary>Determines whether any element of a sequence satisfies a business rule.</summary>
    /// <param name="source">An <see cref="IEnumerable{T}"/> whose elements to apply the business rule to.</param>
    /// <param name="specification">A specification to test each element for a business rule.</param>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <returns><see langword="true" /> if the source sequence is not empty and at least one of its elements passes the test in the specified business rule; otherwise, <see langword="false" />.</returns>
    public static bool Any<T>(this IEnumerable<T> source, Specification<T> specification) =>
        source.Any(specification.IsSatisfiedBy);

    /// <summary>Returns a number that represents how many elements in the specified sequence satisfy a business rule.</summary>
    /// <param name="source">A sequence that contains elements to be tested and counted.</param>
    /// <param name="specification">A specification to test each element for a business rule.</param>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <exception cref="OverflowException">The number of elements in <paramref name="source" /> is larger than <see cref="Int32.MaxValue" />.</exception>
    /// <returns>A number that represents how many elements in the sequence satisfy the business rule.</returns>
    public static int Count<T>(this IEnumerable<T> source, Specification<T> specification) =>
        source.Count(specification.IsSatisfiedBy);

    /// <summary>Returns the first element in a sequence that satisfies a specified business rule.</summary>
    /// <param name="source">An <see cref="IEnumerable{T}"/> to return an element from.</param>
    /// <param name="specification">A specification to test each element for a business rule.</param>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <returns>The first element in the sequence that passes the test in the specified business rule.</returns>
    /// <exception cref="InvalidOperationException">No element satisfies the business rule in the specification. -or- The source sequence is empty.</exception>
    public static T First<T>(this IEnumerable<T> source, Specification<T> specification) =>
        source.First(specification.IsSatisfiedBy);

    /// <summary>Returns the first element of the sequence that satisfies a business rule or a default value if no such element is found.</summary>
    /// <param name="source">An <see cref="IEnumerable{T}" /> to return an element from.</param>
    /// <param name="specification">A specification to test each element for a business rule.</param>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <returns>
    /// <see langword="default" />(<typeparamref name="T" />) if <paramref name="source" /> is empty or if no element
    /// passes the test specified by <paramref name="specification" />; otherwise, the first element in
    /// <paramref name="source" /> that passes the test specified by <paramref name="specification" />.
    /// </returns>
    public static T? FirstOrDefault<T>(this IEnumerable<T> source, Specification<T> specification) =>
        source.FirstOrDefault(specification.IsSatisfiedBy);

    /// <summary>Returns the only element of a sequence that satisfies a business rule, and throws an exception if more than one such element exists.</summary>
    /// <param name="source">An <see cref="IEnumerable{T}" /> to return a single element from.</param>
    /// <param name="specification">A specification to test each element for a business rule.</param>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <exception cref="InvalidOperationException">
    /// No element satisfies the business rule in <paramref name="specification" />. -or-
    /// More than one element satisfies the business rule in <paramref name="specification" />. -or-
    /// The source sequence is empty.
    /// </exception>
    /// <returns>The single element of the input sequence that satisfies a business rule.</returns>
    [SuppressMessage("Naming", "CA1720:Identifiers should not contain type names")]
    public static T Single<T>(this IEnumerable<T> source, Specification<T> specification) =>
        source.Single(specification.IsSatisfiedBy);

    /// <summary>
    /// Returns the only element of a sequence that satisfies a specified business rule or a default value if no such
    /// element exists; this method throws an exception if more than one element satisfies the condition.
    /// </summary>
    /// <param name="source">An <see cref="IEnumerable{T}" /> to return a single element from.</param>
    /// <param name="specification">A specification to test each element for a business rule.</param>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <exception cref="InvalidOperationException">More than one element satisfies the business rule in <paramref name="specification" />.</exception>
    /// <returns>The single element of the input sequence that satisfies the business rule, or <see langword="default" />(<typeparamref name="T" />) if no such element is found.</returns>
    public static T? SingleOrDefault<T>(this IEnumerable<T> source, Specification<T> specification) =>
        source.SingleOrDefault(specification.IsSatisfiedBy);

    /// <summary>Filters a sequence of values based on a specification.</summary>
    /// <param name="source">An <see cref="IEnumerable{T}"/> to filter.</param>
    /// <param name="specification">A specification to test each element for a business rule.</param>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence that satisfy the business rule.</returns>
    public static IEnumerable<T> Where<T>(this IEnumerable<T> source, Specification<T> specification) =>
        source.Where(specification.IsSatisfiedBy);

    /// <summary>Filters a sequence of values based on a specification.</summary>
    /// <param name="queryable">An <see cref="IQueryable{T}"/> to filter.</param>
    /// <param name="specification">A specification to test each element for a business rule.</param>
    /// <typeparam name="T">The type of the elements of source.</typeparam>
    /// <returns>An <see cref="IQueryable{T}"/> that contains elements from the input sequence that satisfy the business rule.</returns>
    public static IQueryable<T> Where<T>(this IQueryable<T> queryable, Specification<T> specification) =>
        queryable.Where(specification.ToExpression());
}
