namespace Raiqub.Expressions.EntityFrameworkCore.Options;

/// <summary>Represents something that configures the querying of an entity.</summary>
public sealed class EntityOptionsConfiguration
{
    private readonly Action<EntityOptions> _configure;

    /// <summary>Initializes a new instance of the <see cref="EntityOptionsConfiguration"/> class.</summary>
    /// <param name="entityType">The type of the entity to configure.</param>
    /// <param name="configure">The action used to configure the options.</param>
    public EntityOptionsConfiguration(Type entityType, Action<EntityOptions> configure)
    {
        EntityType = entityType;
        _configure = configure;
    }

    /// <summary>The type of the entity that this options configures.</summary>
    public Type EntityType { get; }

    /// <summary>Configure an <see cref="EntityOptions"/> instance.</summary>
    /// <param name="options">The options instance to configure.</param>
    public void Configure(EntityOptions options) => _configure(options);
}
