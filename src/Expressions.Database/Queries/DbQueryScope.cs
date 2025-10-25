using System.Collections;

namespace Raiqub.Expressions.Queries;

/// <summary>Represents scope information for database queries, tracking the query modifier, entity, and result types.</summary>
public readonly struct DbQueryScope : IReadOnlyList<KeyValuePair<string, object?>>
{
    private readonly QueryScopeType _scopeType;
    private readonly Type? _modifierType;
    private readonly Type? _entityType;
    private readonly Type _resultType;

    private DbQueryScope(
        QueryScopeType scopeType,
        Type? modifierType,
        Type? entityType,
        Type resultType
    )
    {
        _scopeType = scopeType;
        _modifierType = modifierType;
        _entityType = entityType;
        _resultType = resultType;
    }

    /// <summary>Creates a query scope for the specified entity type without any query modifiers.</summary>
    /// <typeparam name="TEntity">The type of entity to query.</typeparam>
    /// <returns>A new <see cref="DbQueryScope"/> instance.</returns>
    public static DbQueryScope Create<TEntity>() =>
        new(
            scopeType: QueryScopeType.NoModifier,
            modifierType: null,
            entityType: typeof(TEntity),
            resultType: typeof(TEntity)
        );

    /// <summary>Creates a query scope for the specified entity type using a specification.</summary>
    /// <typeparam name="TEntity">The type of entity to query.</typeparam>
    /// <param name="specification">The specification to apply to the query.</param>
    /// <returns>A new <see cref="DbQueryScope"/> instance.</returns>
    public static DbQueryScope Create<TEntity>(Specification<TEntity> specification) =>
        new(
            scopeType: QueryScopeType.Specification,
            modifierType: specification.GetType(),
            entityType: typeof(TEntity),
            resultType: typeof(TEntity)
        );

    /// <summary>Creates a query scope for the specified entity type using an entity query strategy.</summary>
    /// <typeparam name="TEntity">The type of entity to query.</typeparam>
    /// <typeparam name="TResult">The type of the query result.</typeparam>
    /// <param name="queryStrategy">The entity query strategy to apply to the query.</param>
    /// <returns>A new <see cref="DbQueryScope"/> instance.</returns>
    public static DbQueryScope Create<TEntity, TResult>(
        IEntityQueryStrategy<TEntity, TResult> queryStrategy
    ) =>
        new(
            scopeType: QueryScopeType.EntityQueryStrategy,
            modifierType: queryStrategy.GetType(),
            entityType: typeof(TEntity),
            resultType: typeof(TResult)
        );

    /// <summary>Creates a query scope using a query strategy.</summary>
    /// <typeparam name="TResult">The type of the query result.</typeparam>
    /// <param name="queryStrategy">The query strategy to apply to the query.</param>
    /// <returns>A new <see cref="DbQueryScope"/> instance.</returns>
    public static DbQueryScope Create<TResult>(IQueryStrategy<TResult> queryStrategy) =>
        new(
            scopeType: QueryScopeType.QueryStrategy,
            modifierType: queryStrategy.GetType(),
            entityType: null,
            resultType: typeof(TResult)
        );

    /// <summary>Gets the number of key-value pairs in the scope.</summary>
    public int Count => 3;

    /// <summary>Gets the key-value pair at the specified index.</summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>The key-value pair at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is out of range.</exception>
    public KeyValuePair<string, object?> this[int index] =>
        index switch
        {
            0 => new("QueryModifierType", _modifierType?.FullName),
            1 => new("QueryEntityType", _entityType?.FullName),
            2 => new("QueryResultType", _resultType?.FullName),
            _ => throw new ArgumentOutOfRangeException(nameof(index), index, null),
        };

    /// <summary>Returns a string representation of the query scope.</summary>
    /// <returns>A string describing the query scope.</returns>
    public override string? ToString() =>
        _scopeType switch
        {
            QueryScopeType.NoModifier => $"Query for '{_entityType?.FullName}' entity",
            QueryScopeType.Specification =>
                $"Query using '{_modifierType?.FullName}' specification",
            QueryScopeType.EntityQueryStrategy =>
                $"Query using '{_modifierType?.FullName}' entity query strategy",
            QueryScopeType.QueryStrategy =>
                $"Query using '{_modifierType?.FullName}' query strategy",
            _ => null,
        };

    /// <summary>Returns an enumerator that iterates through the key-value pairs in the scope.</summary>
    /// <returns>An enumerator for the scope's key-value pairs.</returns>
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
            yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private enum QueryScopeType
    {
        NoModifier = 1,
        Specification,
        EntityQueryStrategy,
        QueryStrategy,
    }
}
