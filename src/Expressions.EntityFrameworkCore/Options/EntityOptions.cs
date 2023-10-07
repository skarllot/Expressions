using Raiqub.Expressions.Sessions;

namespace Raiqub.Expressions.EntityFrameworkCore.Options;

/// <summary>
/// The options to be used to query for entities of the type specified by <see cref="EntityType"/>.
/// </summary>
public sealed class EntityOptions
{
    /// <summary>Initializes a new instance of the <see cref="EntityOptions"/> class.</summary>
    /// <param name="entityType">The type of the entity to configure.</param>
    public EntityOptions(Type entityType) => EntityType = entityType;

    /// <summary>The type of the entity that this options configures.</summary>
    public Type EntityType { get; }

    /// <summary>Determines whether a query should be split into multiple SQL queries, instead of JOINs.</summary>
    /// <remarks>See: <a href="https://learn.microsoft.com/en-us/ef/core/querying/single-split-queries">Single vs. Split Queries</a>.</remarks>
    public bool UseSplitQuery { get; set; }

    /// <summary>The change tracking mode for the entity.</summary>
    public ChangeTracking? ChangeTracking { get; set; }
}
