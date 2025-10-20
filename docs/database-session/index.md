# Database Session

The database session interfaces abstracts away database operations and integrates with specifications and query strategies.

The **\`Raiqub.Expressions.Reading\`** package provides abstractions for querying data and **\`Raiqub.Expressions.Writing\`** package provides abstractions for saving data.

You should add the _Raiqub.Expressions.Reading_ library on projects that only query data (read) and the _Raiqub.Expressions.Writing_ library on projects that query and save data (read and write).

::: code-group

```shell [.NET CLI]
dotnet add package Raiqub.Expressions.Writing
```

```powershell [Powershell]
PM> Install-Package Raiqub.Expressions.Writing
```

```shell [Paket]
paket add nuget Raiqub.Expressions.Writing
```

:::

You need to register a implementation for database sessions, and the Raiqub Expressions provides implementation for [Entity Framework Core](/ef-core/) and [Marten](/marten/). If you need to use another ORM library, you will need to implement your own database session factory and database session implementing **\`IDbSessionFactory\`** and **\`IDbSession\`** interfaces.

::: code-group

```csharp [EF Core]
services.AddEntityFrameworkExpressions()
    .AddSingleContext<YourDbContext>();
```

```csharp [Marten]
services.AddMartenExpressions()
    .AddSingleContext();
```

:::

## Injecting Database Session

Inject the appropriate session interface (`IDbQuerySession` for read sessions, `IDbSession` for read and write sessions) into your services, and use it read and write from/to database.

::: code-group

```csharp [C# classic]
public class YourService
{
    private readonly IDbSession _dbSession;

    public YourService(IDbSession dbSession)
    {
        _dbSession = dbSession;
    }

    // ...
}
```

```csharp [C# 12]
public class YourService(IDbSession dbSession)
{
    // ...
}
```

:::

## Creating Query Sessions and Querying Data

To create a query session and query data using a query strategy, follow these steps:

1. Inject an instance of `IDbQuerySessionFactory` into your service or controller.
2. Use the `Create()` method of the `IDbQuerySessionFactory` interface to create a new query session.
3. Call the `Query()` method on the query session, passing in your query strategy or specification instance.
4. Call one of the methods on the resulting `IDbQuery<T>` interface to execute the query and retrieve the results.

```csharp
await using (var session = querySessionFactory.Create())
{
    IDbQuery<Customer> query = session.Query(new CustomerIsActive());
    IReadOnlyList<Customer> customers = await query.ToListAsync();
}
```

### Querying Value Types

When your query strategy returns value types (structs), you should use the `QueryValue()` method instead of `Query()`. This method returns an `IDbQueryValue<T>` interface which provides nullable return values for optional operations like `FirstOrDefaultAsync()` and `SingleOrDefaultAsync()`.

The `QueryValue()` method has two overloads:

- **`QueryValue<TEntity, TResult>(IEntityQueryStrategy<TEntity, TResult>)`** - For entity-based queries that return value types
- **`QueryValue<TResult>(IQueryStrategy<TResult>)`** - For multi-entity queries that return value types

::: code-group

```csharp [Entity Query]
await using (var session = querySessionFactory.Create())
{
    // Query strategy that returns a value type (e.g., int, decimal, DateTime)
    IDbQueryValue<int> query = session.QueryValue(new GetActiveCustomerCount());

    // Returns null if no result is found
    int? count = await query.FirstOrDefaultAsync();
}
```

```csharp [Multi-Entity Query]
await using (var session = querySessionFactory.Create())
{
    // Query strategy that joins multiple entities and returns a value type
    IDbQueryValue<decimal> query = session.QueryValue(new GetTotalOrderValue());

    // Returns the single value or null, throws if more than one result
    decimal? total = await query.SingleOrDefaultAsync();
}
```

:::

**Key differences between `IDbQuery<T>` and `IDbQueryValue<T>`:**

- `IDbQuery<T>` is for reference types (classes) and returns non-nullable results for `FirstOrDefaultAsync()` and `SingleOrDefaultAsync()`
- `IDbQueryValue<T>` is for value types (structs) and returns nullable results (`T?`) for `FirstOrDefaultAsync()` and `SingleOrDefaultAsync()`
- Both interfaces support `ToListAsync()`, `CountAsync()`, and other query operations

## Creating Write Sessions and Writing Data

To create a write session and write data to the database, follow these steps:

1. Inject an instance of `IDbSessionFactory` into your service or controller.
2. Use the `Create()` method of the `IDbSessionFactory` interface to create a new write session.
3. Call the appropriate methods on the write session to perform insert, update, or delete operations on your entities.
4. Call the `SaveChangesAsync()` method on the write session to persist your changes to the database.

```csharp
await using (var session = sessionFactory.Create())
{
    var blog = new Blog { Url = "https://example.com" };
    session.Add(blog);
    await session.SaveChangesAsync();
}
```