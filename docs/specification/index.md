# Specification

The Specification Pattern is a behavioral design pattern used to encapsulate business rules into composable, reusable and testable objects. This pattern is often used in domains where queries or validation rules need to be expressed in a more readable and maintainable form.

A specification, in the context of this package, is an object that defines a condition that must be satisfied by elements of a certain type. These conditions can be as simple or as complex as needed and are expressed using lambda expressions.

The **\`Raiqub.Expressions\`** package provides the `Specification<T>` base class for creating specifications. It is optimized to allow ORM frameworks to evaluate and translate it into SQL queries.

To add Raiqub.Expressions library to a .NET project, go get it from Nuget:

::: code-group

```shell [.NET CLI]
dotnet add package Raiqub.Expressions
```

```powershell [Powershell]
PM> Install-Package Raiqub.Expressions
```

```shell [Paket]
paket add nuget Raiqub.Expressions
```

:::

## Creating a Simple Specification

Here's a basic example of a specification that checks if a product is in stock:

```csharp
public class ProductIsInStock : Specification<Product>
{
    public override Expression<Func<Product, bool>> ToExpression()
    {
        return product => product.AvailableQuantity > 0;
    }
}
```

## Creating Specification Factories

You can also create specification factories, which are static classes that provide predefined specifications. Here's an example of a specification factory for products:

```csharp
public static class ProductSpecification
{
    public static Specification<Product> IsInStock { get; } =
        Specification.Create<Product>(product => product.AvailableQuantity > 0);

    public static Specification<Product> IsDiscountAvailable(DateTimeOffset now) =>
        Specification.Create<Product>(product => product.DiscountStartDate <= now && now <= product.DiscountEndDate);
}
```

## Combining Specifications

Specifications can be combined using extension methods or logical operators to create more complex conditions. This flexibility allows you to express intricate business rules concisely. Here are examples of combining specifications for incidents:

::: code-group

```csharp [Extension Methods]
    public static Specification<Incident> IsClosed { get; } =
        Specification.Create<Incident>(incident => incident.Status == IncidentStatus.Closed);

    public static Specification<Incident> IsResolved { get; } =
        Specification.Create<Incident>(incident => incident.Status == IncidentStatus.Resolved);

    public static Specification<Incident> IsNotResolved { get; } =
        IsResolved.Not();

    public static Specification<Incident> IsResolvedOrClosed { get; } =
        IsResolved.Or(IsClosed);
```

```csharp [Logical Operators]
    public static Specification<Incident> IsClosed { get; } =
        Specification.Create<Incident>(incident => incident.Status == IncidentStatus.Closed);

    public static Specification<Incident> IsResolved { get; } =
        Specification.Create<Incident>(incident => incident.Status == IncidentStatus.Resolved);

    public static Specification<Incident> IsNotResolved { get; } =
        !IsResolved;

    public static Specification<Incident> IsResolvedOrClosed { get; } =
        IsResolved | IsClosed;
```

:::

## Practical Use Cases

Specifications are valuable for filtering data, composing complex queries, and validating entities against business rules. They are particularly useful when working with Object-Relational Mapping (ORM) frameworks, as they can be translated into SQL queries for efficient database operations.