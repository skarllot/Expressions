# Migration Guide

## Key Changes in 2.2
In the version 2.2, extension methods for `IDbSession` and `IDbQuery` were added to respective interface contract. And `IEntityQueryStrategy` interface no longer extends the `IQueryStrategy` interface.

## Key Changes in 2.0

In the version 2.0, the `IQueryModel`-related interfaces and classes was renamed to `IQueryStrategy`. These changes are summarized in the table below:

| V1                                       | V2                                          |
|------------------------------------------|---------------------------------------------|
| IQueryModel&lt;TResult&gt;               | IQueryStrategy&lt;TResult&gt;               |
| IEntityQueryModel&lt;TResult&gt;         | IEntityQueryStrategy&lt;TResult&gt;         |
| EntityQueryModel&lt;TSource, TResult&gt; | EntityQueryStrategy&lt;TSource, TResult&gt; |
| QueryModel                               | QueryStrategy                               |

We chose to rename these interfaces and classes to use the "Strategy" suffix (`IQueryStrategy`, `IEntityQueryStrategy`, etc.) to better reflect their purpose. This new naming convention aligns with design patterns like the Strategy Pattern, which emphasizes encapsulating algorithms and making them interchangeable.

It's important to note that the `IEntityQueryStrategy` interface now extends the `IQueryStrategy` interface. This hierarchy reflects the relationship between entity-specific query strategies and general query strategies. You can use this inheritance structure to create custom query strategies that build upon the foundation provided by `IQueryStrategy`.