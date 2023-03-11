namespace Raiqub.Expressions.Repositories;

/// <summary>Defines how change tracker will handle returned entities.</summary>
public enum ChangeTracking
{
    /// <summary>Default behavior.</summary>
    Default,
    /// <summary>Keep track of changes of returned entities.</summary>
    Enable,
    /// <summary>Not track changes of returned entities, but use same instance for entities with same key.</summary>
    IdentityResolution,
    /// <summary>Not track changes of returned entities.</summary>
    Disable
}
