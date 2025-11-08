# Migration Guide

## Key Changes in 3.0

Version 3.0 introduces several important changes to framework support:

### Framework Support Changes

- **.NET 6 support has been dropped**: All libraries no longer support .NET 6
- **Expressions, Expressions.Reading, and Expressions.Writing** remain compatible with .NET Standard 2.0 and 2.1
- **Expressions.EntityFrameworkCore and Expressions.Marten** libraries now require .NET 8 or higher
- **Expressions.Database library has been removed**: The Database library is no longer available in version 3.0

### Backward Compatibility

While you can use Expressions.EntityFrameworkCore or Expressions.Marten libraries from version 2 alongside Expressions,
Expressions.Reading, and Expressions.Writing libraries from version 3, compatibility is not guaranteed in future releases. We recommend upgrading all libraries to version 3 when possible.

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
