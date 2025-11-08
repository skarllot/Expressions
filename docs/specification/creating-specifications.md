# Creating Specifications

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
