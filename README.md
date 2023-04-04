# Expressions

[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](https://raw.githubusercontent.com/EngRajabi/Enum.Source.Generator/master/LICENSE) [![Nuget](https://img.shields.io/nuget/v/Raiqub.Expressions)](https://www.nuget.org/packages/Raiqub.Expressions) [![Nuget](https://img.shields.io/nuget/dt/Raiqub.Expressions?label=Nuget.org%20Downloads&style=flat-square&color=blue)](https://www.nuget.org/packages/Raiqub.Expressions)

_Raiqub.Expressions is a library that provides abstractions for creating specifications and query models using LINQ expressions. It also supports querying and writing to databases using various providers._

<hr />

## Features
* Abstractions for creating specifications and query models
* Abstractions for querying and writing to databases
* Supports Entity Framework Core and Marten providers
* Built with .NET Standard 2.0, 2.1, and .NET Core 6.0

## NuGet Packages
* **Raiqub.Expressions**: abstractions for creating specifications and query models (defines Specification and QueryModel base classes)
* **Raiqub.Expressions.Reading**: abstractions for creating query sessions and querying from database (defines IQuerySessionFactory and IQuerySession interfaces)
* **Raiqub.Expressions.Writing**: abstractions for creating write sessions and writing to database (defines ISessionFactory and ISession interfaces)
* **Raiqub.Expressions.EntityFrameworkCore**: implementation of sessions and factories using Entity Framework Core
* **Raiqub.Expressions.Marten**: implementation of sessions and factories using Marten library

## Usage
To use the library, you can install the desired NuGet package(s) in your project and start creating your specifications and query models. Here's an example of how to create a simple specification:

```csharp
public class CustomerIsActiveSpecification : Specification<Customer>
{
    public override Expression<Func<Customer, bool>> ToExpression()
    {
        return customer => customer.IsActive;
    }
}
```
And here's an example of how to use the specification with Entity Framework Core:

```csharp
await using (var session = querySessionFactory.Create())
{
    var query = session.Query(new CustomerIsActiveSpecification());
    var customers = await query.ToListAsync();
}
```

## Contributing

If something is not working for you or if you think that the source file
should change, feel free to create an issue or Pull Request.
I will be happy to discuss and potentially integrate your ideas!

## License

This library is licensed under the [MIT License](./LICENSE).
