# Expressions

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](https://raw.githubusercontent.com/EngRajabi/Enum.Source.Generator/master/LICENSE) [![Nuget](https://img.shields.io/nuget/v/Raiqub.Expressions)](https://www.nuget.org/packages/Raiqub.Expressions) [![Nuget](https://img.shields.io/nuget/dt/Raiqub.Expressions?label=Nuget.org%20Downloads&style=flat-square&color=blue)](https://www.nuget.org/packages/Raiqub.Expressions)

_Raiqub.Expressions is a library that provides abstractions for creating specifications and query strategies using LINQ expressions. It also supports querying and writing to databases using various providers._

[🏃 Quickstart](#quickstart) &nbsp; | &nbsp; [📗 Guide](#guide) &nbsp; | &nbsp; [🔄 Migration](#migration-guide)

<hr />

## Features
* Abstractions for creating specifications and query strategies
* Abstractions for querying and writing to databases
* Supports Entity Framework Core and Marten providers
* Built with .NET Standard 2.0, 2.1, and .NET Core 6.0

## NuGet Packages
* **Raiqub.Expressions**: abstractions for creating specifications
* **Raiqub.Expressions.Reading**: abstractions for creating query strategies and query sessions and querying from database (defines IDbQuerySessionFactory and IDbQuerySession interfaces)
* **Raiqub.Expressions.Writing**: abstractions for creating write sessions and writing to database (defines IDbSessionFactory and IDbSession interfaces)
* **Raiqub.Expressions.EntityFrameworkCore**: implementation of sessions and factories using Entity Framework Core
* **Raiqub.Expressions.Marten**: implementation of sessions and factories using Marten library

## Prerequisites
Before you begin, you'll need the following:

* .NET Standard 2.0 or 2.1, or .NET Core 6.0 installed on your machine
* An IDE such as Visual Studio, Visual Studio Code, or JetBrains Rider
* A database to query against (if using the reading package) or write to (if using the writing package)

## Quickstart

To use Raiqub.Expressions in your project, follow these steps:

### Entity Framework Core

1. Install the required NuGet package(s) for the database provider you'll be using, such as **\`Microsoft.EntityFrameworkCore.SqlServer\`**

2. Install the **\`Raiqub.Expressions.EntityFrameworkCore\`** NuGet package

3. Register your DbContext by using **\`AddDbContextFactory\`** extension method

    ```csharp
    services.AddDbContextFactory<YourDbContext>();
    ```

4. Register the session and session factories using the appropriate extension method(s) for your database provider:

    ```csharp
    services.AddEntityFrameworkExpressions()
        .AddSingleContext<YourDbContext>();
    ```

### Marten

1. Install the **\`Marten\`** NuGet package

2. Install the **\`Raiqub.Expressions.Marten\`** NuGet package

3. Register the session and session factories using the appropriate extension method(s) for your database provider:

    ```csharp
    services.AddMartenExpressions()
        .AddSingleContext();
    ```

### Using

Inject the appropriate session interface (**\`IDbQuerySession\`** for read sessions, **\`IDbSession\`** for write sessions) into your services, and use it read and write from/to database.

You can also create specifications and query strategies. Here's an example of how to create a simple specification:

```csharp
public class CustomerIsActive : Specification<Customer>
{
    public override Expression<Func<Customer, bool>> ToExpression()
    {
        return customer => customer.IsActive;
    }
}
```
And here's an example of how to use the specification:

```csharp
// session is of type IDbSession or IDbQuerySession and can be injected
var query = session.Query(new CustomerIsActive());
var customers = await query.ToListAsync();
```

## Guide

### Creating Specifications
The Specification Pattern is a behavioral design pattern used to encapsulate business rules into composable, reusable and testable objects. This pattern is often used in domains where queries or validation rules need to be expressed in a more readable and maintainable form.

The **\`Raiqub.Expressions\`** package provides the **\`Specification&lt;T&gt;**\` base class for creating specifications. It is optimized to allow ORM frameworks to evaluate and translate it into SQL queries.

Here's an example of creating a specification by inheriting from **\`Specification&lt;T&gt;**\` base class:

```csharp
public class ProductIsInStock : Specification<Product>
{
    public override Expression<Func<Product, bool>> ToExpression()
    {
        return product => product.AvailableQuantity > 0;
    }
}
```

or, you can create a static class to provide the specifications of an object:

```csharp
public static class ProductSpecification
{
    public static Specification<Product> IsInStock { get; } =
        Specification.Create<Product>(product => product.AvailableQuantity > 0);

    public static Specification<Product> IsDiscountAvailable(DateTime now) =>
        Specification.Create<Product>(product => product.DiscountStartDate <= now && now <= product.DiscountEndDate);
}
```

The specifications can be combined using the available extension methods or the logical operators:

```csharp
    public static Specification<Incident> IsClosed { get; } =
        Specification.Create<Incident>(incident => incident.Status == IncidentStatus.Closed);

    public static Specification<Incident> IsResolved { get; } =
        Specification.Create<Incident>(incident => incident.Status == IncidentStatus.Resolved);

    // =======================
    // Using extension methods
    // =======================
    public static Specification<Incident> IsNotResolved { get; } =
        IsResolved.Not();

    public static Specification<Incident> IsResolvedOrClosed { get; } =
        IsResolved.Or(IsClosed);

    // =======================
    // Using logical operators
    // =======================
    public static Specification<Incident> IsNotResolved { get; } =
        !IsResolved;

    public static Specification<Incident> IsResolvedOrClosed { get; } =
        IsResolved | IsClosed;
```

### Creating Query Strategies
The query strategy is based on the Strategy Pattern by defining a strategy for querying the database allowing better concern separation, maintainability and reusability than the repository pattern.

The **\`Raiqub.Expressions.Reading\`** package provides abstractions for creating query strategies. You can create a new query strategy by choosing one of several ways available to implement a query strategy.

#### Single Entity Query
The most common strategy is querying a single entity and for that purpose the interface **\`IEntityQueryStrategy&lt;TSource, TResult&gt;\`** was created and its abstract class implementation **\`EntityQueryStrategy&lt;TSource, TResult&gt;\`**.

Here's an example of a entity query strategy that filters a list of entities based on a set of conditions:

```csharp
public class GetProductNameQueryStrategy : EntityQueryStrategy<Product, ProductName>
{
    protected override IQueryable<ProductName> ExecuteCore(IQueryable<Product> source)
    {
        return source
            .Where(ProductSpecification.IsInStock)
            .OrderBy(e => e.Name)
            .Select(e => new ProductName { Id = e.Id, Name = e.Name });
    }
}
```

or, you can define only the preconditions:

```csharp
public class GetProductInStockQueryStrategy : EntityQueryStrategy<Product>
{
    protected override IEnumerable<Specification<Product>> GetPreconditions()
    {
        yield new ProductIsInStock();
    }
}
```

or yet, you can create a static class as a provider of query strategies:

```csharp
public static class ProductQueryStrategy
{
    public static IEntityQueryStrategy<Product, ProductName> GetName() =>
        QueryStrategy.CreateForEntity(
            (IQueryable<Product> source) => source
                .Where(ProductSpecification.IsInStock)
                .OrderBy(e => e.Name)
                .Select(e => new ProductName { Id = e.Id, Name = e.Name });
}
```

#### Multiple Entities Query
For the cases where multiple entities need to be queried the interface **\`IQueryStrategy&lt;TResult&gt;\`** was created.

You can implement the interface directly, as the example below:

```csharp
public class GetProductNameOfOpenStoreQueryStrategy : IQueryStrategy<ProductName>
{
    public IQueryable<TResult> Execute(IQuerySource source) =>
        source => from product in source.GetSetUsing(ProductSpecification.IsInStock)
            join store in source.GetSetUsing(StoreSpecification.IsOpen) on
                product.StoreId equals store.Id
            orderby product.Name
            select new ProductName { Id = e.Id, Name = e.Name };
}
```

or, can create a static class as a provider of query strategies:

```csharp
public static class ProductQueryStrategy
{
    public static IQueryStrategy<ProductName> GetNameOfOpenStore() =>
        QueryStrategy.Create(
            source => from product in source.GetSetUsing(ProductSpecification.IsInStock)
                join store in source.GetSetUsing(StoreSpecification.IsOpen) on
                    product.StoreId equals store.Id
                orderby product.Name
                select new ProductName { Id = e.Id, Name = e.Name });
}
```

### Creating Query Sessions and Querying Data
To create a query session and query data using a query strategy, follow these steps:

1. Inject an instance of **\`IDbQuerySessionFactory\`** into your service or controller.
2. Use the **\`Create()\`** method of the **\`IDbQuerySessionFactory\`** interface to create a new query session.
3. Call the **\`Query()\`** method on the query session, passing in your query strategy or specification instance.
4. Call one of the methods on the resulting **\`IDbQuery&lt;T&gt;\`** interface to execute the query and retrieve the results.

```csharp
await using (var session = querySessionFactory.Create())
{
    IDbQuery<Customer> query = session.Query(new CustomerIsActive());
    IReadOnlyList<Customer> customers = await query.ToListAsync();
}
```

### Creating Write Sessions and Writing Data
To create a write session and write data to the database, follow these steps:

1. Inject an instance of **\`IDbSessionFactory\`** into your service or controller.
2. Use the **\`Create()\`** method of the **\`IDbSessionFactory\`** interface to create a new write session.
3. Call the appropriate methods on the write session to perform insert, update, or delete operations on your entities.
4. Call the **\`SaveChangesAsync()\`** method on the write session to persist your changes to the database.

```csharp
await using (var session = sessionFactory.Create())
{
    var blog = new Blog { Url = "https://example.com" };
    session.Add(blog);
    await session.SaveChangesAsync();
}
```

### Defining custom SQL query for entity (Entity Framework)
To define a custom SQL query for retrieving a entity from database, follow these steps:

1. Create a new class implementing the **\`ISqlProvider\<TEntity\>\`** interface;
2. Implement the **\`GetQuerySql()\`** method returning a raw or an interpolated SQL string;
3. Register the created class as a implementation of _ISqlProvider_ interface for dependency injection using the singleton lifetime.

Example implementing a custom SQL query for _Blog_ entity:

```csharp
private class BlogSqlProvider : ISqlProvider<Blog>
{
    public SqlString GetQuerySql() => SqlString.FromSqlInterpolated($"SELECT \"Id\", \"Name\" FROM \"Blog\"");
}
```

And then, registering it:

```csharp
services.AddSingleton<ISqlProvider, BlogSqlProvider>();
```

### Supported Databases
Currently, Raiqub.Expressions supports the following ORM libraries:
* Entity Framework Core
* Marten

If you need to use another ORM library, you will need to implement your own database session factory and database session implementing **\`IDbSessionFactory\`** and **\`IDbSession\`** interfaces.

## Migration Guide

### Key Changes in 2.0.0
The V2 release renamed the **\`IQueryModel\`**-related interfaces and classes to **\`IQueryStrategy\`**. This is the list of relevant renames:

| V1                                      | V2                                         |
|-----------------------------------------|--------------------------------------------|
| IQueryModel&ltTResult&gt;               | IQueryStrategy&ltTResult&gt;               |
| IEntityQueryModel&ltTResult&gt;         | IEntityQueryStrategy&ltTResult&gt;         |
| EntityQueryModel&ltTSource, TResult&gt; | EntityQueryStrategy&ltTSource, TResult&gt; |
| QueryModel                              | QueryStrategy                              |

Additionally, the **\`IEntityQueryStrategy\`** interface extends the **\`IQueryStrategy\`** interface.

## Contributing

If something is not working for you or if you think that the source file
should change, feel free to create an issue or Pull Request.
I will be happy to discuss and potentially integrate your ideas!

## License

This library is licensed under the [MIT License](./LICENSE).
