namespace Raiqub.Expressions.EntityFrameworkCore.Options;

/// <summary>An selector that retrieves an <see cref="EntityOptions"/> based on a specified entity type.</summary>
public sealed class EntityOptionsSelector
{
    public static readonly EntityOptionsSelector Empty = new(Array.Empty<EntityOptionsConfiguration>());

    private readonly Dictionary<Type, EntityOptions> _dictionary;

    public EntityOptionsSelector(IEnumerable<EntityOptionsConfiguration> configurations)
    {
        _dictionary = configurations
            .GroupBy(static x => x.EntityType)
            .ToDictionary(
                static x => x.Key,
                static x => x.Aggregate(
                    new EntityOptions(x.Key),
                    static (acc, curr) =>
                    {
                        curr.Configure(acc);
                        return acc;
                    }));
    }

    /// <summary>Gets the <see cref="EntityOptions"/> for the specified entity type</summary>
    /// <typeparam name="TEntity">The type of the entity to get options.</typeparam>
    /// <returns>The <see cref="EntityOptions"/> instance, or <c>null</c> if it is not defined.</returns>
    public EntityOptions? GetOptions<TEntity>() where TEntity : class
    {
        return _dictionary.TryGetValue(typeof(TEntity), out EntityOptions? value)
            ? value
            : null;
    }
}
