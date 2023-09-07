# Expressions

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](https://raw.githubusercontent.com/EngRajabi/Enum.Source.Generator/master/LICENSE) [![Nuget](https://img.shields.io/nuget/v/Raiqub.Expressions)](https://www.nuget.org/packages/Raiqub.Expressions) [![Nuget](https://img.shields.io/nuget/dt/Raiqub.Expressions?label=Nuget.org%20Downloads&style=flat-square&color=blue)](https://www.nuget.org/packages/Raiqub.Expressions)

_Raiqub.Expressions is a library that provides abstractions for creating specifications and query models using LINQ expressions. It also supports querying and writing to databases using various providers._

[🏃 Quickstart](#quickstart) &nbsp; | &nbsp; [📗 Guide](#guide) &nbsp; | &nbsp; [📦 NuGet](https://www.nuget.org/packages/Raiqub.Expressions)

<hr />

## Features
* Abstractions for creating specifications and query models
* Abstractions for querying and writing to databases
* Supports Entity Framework Core and Marten providers
* Built with .NET Standard 2.0, 2.1, and .NET Core 6.0

## NuGet Packages
* **Raiqub.Expressions**: abstractions for creating specifications
* **Raiqub.Expressions.Reading**: abstractions for creating query sessions and querying from database (defines IDbQuerySessionFactory and IDbQuerySession interfaces)
* **Raiqub.Expressions.Writing**: abstractions for creating write sessions and writing to database (defines IDbSessionFactory and IDbSession interfaces)
* **Raiqub.Expressions.EntityFrameworkCore**: implementation of sessions and factories using Entity Framework Core
* **Raiqub.Expressions.Marten**: implementation of sessions and factories using Marten library

## Prerequisites
Before you begin, you'll need the following:

* .NET Standard 2.0 or 2.1, or .NET Core 6.0 installed on your machine
* An IDE such as Visual Studio, Visual Studio Code, or JetBrains Rider
* A database to query against (if using the reading package) or write to (if using the writing package)

## Quickstart
To use the library, you can install the desired NuGet package(s) in your project and start creating your specifications and query models. Here's an example of how to create a simple specification:

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

To use Raiqub.Expressions in your project, follow these steps:

1. Install the required NuGet package(s) for the database provider you'll be using, such as **\`Microsoft.EntityFrameworkCore.SqlServer\`** for Entity Framework Core or **\`Marten\`** for Marten.

2. Install the **\`Raiqub.Expressions.EntityFrameworkCore\`** package if using Entity Framework or the **\`Raiqub.Expressions.Marten\`** package if using Marten.

3. Register the session and session factories using the appropriate extension method(s) for your database provider:

    For Entity Framework Core:

    ```csharp
    // The DbContext must be registered calling AddDbContextFactory<YourDbContext>()
    services.AddEntityFrameworkExpressions()
        .AddSingleContext<YourDbContext>();
    ```

    For Marten:

    ```csharp
    services.AddMartenExpressions()
        .AddSingleContext();
    ```

4. Inject the appropriate session interface (IDbQuerySession for read sessions, IDbSession for write sessions) into your services, and use it read and write from/to database.

### Creating Query Models and Specifications
The **\`Raiqub.Expressions\`** package provides abstractions for creating specifications and query models. You can create a new query model by creating a new class that derives from **\`QueryModel&lt;TSource, TResult&gt;**\`. Similarly, you can create a new specification by creating a new class that derives from **\`Specification&lt;T&gt;**\`.

Here's an example of a query model that filters a list of entities based on a set of conditions:

```csharp
public class MyQueryModel : QueryModel<MyEntity, MyResult>
{
    protected override IEnumerable<Specification<MyEntity>> GetPreconditions()
    {
        yield return new MyEntityIsEnabledSpecification();
    }

    protected override IQueryable<MyResult> ExecuteCore(IQueryable<MyEntity> source)
    {
        return source.OrderBy(e => e.Name).Select(e => new MyResult { Id = e.Id, Name = e.Name });
    }
}
```

And here's an example of a specification that checks if an entity is enabled:

```csharp
public class MyEntityIsEnabled : Specification<MyEntity>
{
    public override Expression<Func<MyEntity, bool>> ToExpression()
    {
        return entity => entity.IsEnabled;
    }
}
```

### Creating Query Sessions and Querying Data
To create a query session and query data using a query model, follow these steps:

1. Inject an instance of **\`IDbQuerySessionFactory\`** into your service or controller.
2. Use the **\`Create()\`** method of the **\`IDbQuerySessionFactory\`** interface to create a new query session.
3. Call the **\`Query()\`** method on the query session, passing in your query model or specification instance.
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
Currently, Raiqub.Expressions supports the following database libraries:
* Entity Framework Core
* Marten

If you need to use another database, you will need to implement your own database session factory and database session implementing **\`IDbSessionFactory\`** and **\`IDbSession\`** interfaces.

## Contributing

If something is not working for you or if you think that the source file
should change, feel free to create an issue or Pull Request.
I will be happy to discuss and potentially integrate your ideas!

## License

This library is licensed under the [MIT License](./LICENSE).
