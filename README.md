# Raiqub Expressions

_Raiqub.Expressions is a library that provides abstractions for creating specifications and query strategies using LINQ expressions. It also supports querying and writing to databases using various providers._

[![Build status](https://github.com/skarllot/Expressions/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/skarllot/Expressions/actions)
[![OpenSSF Scorecard](https://api.securityscorecards.dev/projects/github.com/skarllot/Expressions/badge)](https://securityscorecards.dev/viewer/?uri=github.com/skarllot/Expressions)
[![Code coverage](https://codecov.io/gh/skarllot/Expressions/branch/main/graph/badge.svg)](https://codecov.io/gh/skarllot/Expressions)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2Fskarllot%2FExpressions%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/skarllot/Expressions/main)
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg?style=flat)](https://raw.githubusercontent.com/skarllot/Expressions/master/LICENSE)

[🏃 Quickstart](#quickstart) &nbsp; | &nbsp; [📖 Documentation](https://fgodoy.me/Expressions/) &nbsp; | &nbsp; [🔄 Migration](https://fgodoy.me/Expressions/migration-guide.html)

<hr />

## Features
* Easily define and compose specifications to encapsulate business rules
* Create custom query strategies for flexible and efficient data retrieval
* Simplify database operations by using consistent abstractions
* Seamlessly integrate with Entity Framework Core for database interactions
* Utilize Marten providers for a NoSQL document database experience
* Built with .NET Standard 2.0, 2.1, and .NET 6.0

## NuGet Packages
* [![NuGet](https://img.shields.io/nuget/v/Raiqub.Expressions?label=&logo=nuget&style=flat-square)![NuGet](https://img.shields.io/nuget/dt/Raiqub.Expressions?label=&style=flat-square)](https://www.nuget.org/packages/Raiqub.Expressions/) **Raiqub.Expressions**: provides abstractions for creating specifications
* [![NuGet](https://img.shields.io/nuget/v/Raiqub.Expressions.Reading?label=&logo=nuget&style=flat-square)![NuGet](https://img.shields.io/nuget/dt/Raiqub.Expressions.Reading?label=&style=flat-square)](https://www.nuget.org/packages/Raiqub.Expressions.Reading/) **Raiqub.Expressions.Reading**: provides abstractions for creating query strategies and query sessions. Defines the `IDbQuerySession` and `IDbQuerySessionFactory` interfaces for querying from the database
* [![NuGet](https://img.shields.io/nuget/v/Raiqub.Expressions.Writing?label=&logo=nuget&style=flat-square)![NuGet](https://img.shields.io/nuget/dt/Raiqub.Expressions.Writing?label=&style=flat-square)](https://www.nuget.org/packages/Raiqub.Expressions.Writing/) **Raiqub.Expressions.Writing**: provides abstractions for creating write sessions and performing write operations. Defines the `IDbSession` and `IDbSessionFactory` interfaces for writing to the database
* [![NuGet](https://img.shields.io/nuget/v/Raiqub.Expressions.EntityFrameworkCore?label=&logo=nuget&style=flat-square)![NuGet](https://img.shields.io/nuget/dt/Raiqub.Expressions.EntityFrameworkCore?label=&style=flat-square)](https://www.nuget.org/packages/Raiqub.Expressions.EntityFrameworkCore/) **Raiqub.Expressions.EntityFrameworkCore**: implements sessions and factories using Entity Framework Core. Ideal for integrating with Entity Framework Core for database access
* [![NuGet](https://img.shields.io/nuget/v/Raiqub.Expressions.Marten?label=&logo=nuget&style=flat-square)![NuGet](https://img.shields.io/nuget/dt/Raiqub.Expressions.Marten?label=&style=flat-square)](https://www.nuget.org/packages/Raiqub.Expressions.Marten/) **Raiqub.Expressions.Marten**: implements sessions and factories using Marten library. Perfect for leveraging Marten's NoSQL document database capabilities

## Documentation
This README aims to give a quick overview of some Raiqub Expressions features. For deeper detail of available features, be sure also to check out [Documentation Page](https://fgodoy.me/Expressions/).

## Prerequisites
Before you begin, you'll need the following:

* .NET Standard 2.0 or 2.1, or .NET Core 6.0 installed on your machine
* An IDE such as Visual Studio, Visual Studio Code, or JetBrains Rider
* If you plan to use the reading package, have a database available for querying. If you intend to use the writing package, ensure you have a writable database to perform write operations

## Quickstart

To use Raiqub.Expressions in your project, follow these steps:

### Entity Framework Core

1. Install the required NuGet package(s) for the database provider you'll be using, such as **\`Microsoft.EntityFrameworkCore.SqlServer\`**

2. Install the **\`Raiqub.Expressions.EntityFrameworkCore\`** NuGet package

3. Register your DbContext by using **\`AddDbContextFactory\`** extension method

    ```csharp
    services.AddDbContextFactory<YourDbContext>();
    ```

4. Register the session and session factories using the following extension method:

    ```csharp
    services.AddEntityFrameworkExpressions()
        .AddSingleContext<YourDbContext>();
    ```

### Marten

1. Install the **\`Marten\`** NuGet package

2. Install the **\`Raiqub.Expressions.Marten\`** NuGet package

3. Register the session and session factories using the following extension method:

    ```csharp
    services.AddMartenExpressions()
        .AddSingleContext();
    ```

### Using

Inject the appropriate session interface (`IDbQuerySession` for read sessions, `IDbSession` for read and write sessions) into your services, and use it read and write from/to database.

```csharp
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
// Assuming 'session' is of type IDbSession or IDbQuerySession and has been injected
var query = session.Query(new CustomerIsActive());
var customers = await query.ToListAsync();
```

## Contributing

If something is not working for you or if you think that the source file
should change, feel free to create an issue or Pull Request.
I will be happy to discuss and potentially integrate your ideas!

## License

This library is licensed under the [MIT License](./LICENSE).
