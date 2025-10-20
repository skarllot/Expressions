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

## Using Specifications with Database Sessions

Once you've defined your specifications, you can use them with database sessions to query data efficiently. Here are practical examples:

::: code-group

```csharp [Query with Specification]
public class ProductService
{
    private readonly IDbQuerySession _session;

    public ProductService(IDbQuerySession session)
    {
        _session = session;
    }

    public async Task<IReadOnlyList<Product>> GetAvailableProductsAsync(
        CancellationToken cancellationToken = default)
    {
        var query = _session.Query(ProductSpecification.IsInStock);
        return await query.ToListAsync(cancellationToken);
    }
}
```

```csharp [Combining Specifications]
public class ProductService
{
    private readonly IDbQuerySession _session;

    public ProductService(IDbQuerySession session)
    {
        _session = session;
    }

    public async Task<IReadOnlyList<Product>> GetDiscountedInStockProductsAsync(
        DateTimeOffset now,
        CancellationToken cancellationToken = default)
    {
        // Combine multiple specifications
        var specification = ProductSpecification.IsInStock
            .And(ProductSpecification.IsDiscountAvailable(now));

        var query = _session.Query(specification);
        return await query.ToListAsync(cancellationToken);
    }
}
```

```csharp [With Query Strategy]
public class GetProductNameQueryStrategy : EntityQueryStrategy<Product, ProductName>
{
    protected override IQueryable<ProductName> ExecuteCore(IQueryable<Product> source)
    {
        // Use specifications within query strategies
        return source
            .Where(ProductSpecification.IsInStock)
            .OrderBy(e => e.Name)
            .Select(e => new ProductName { Id = e.Id, Name = e.Name });
    }
}
```

:::

## Common Patterns

### Parameterized Specifications

Create specifications that accept parameters to make them more flexible and reusable:

```csharp
public static class ProductSpecification
{
    public static Specification<Product> HasMinimumQuantity(int minimumQuantity) =>
        Specification.Create<Product>(p => p.AvailableQuantity >= minimumQuantity);

    public static Specification<Product> InPriceRange(decimal minPrice, decimal maxPrice) =>
        Specification.Create<Product>(p => p.Price >= minPrice && p.Price <= maxPrice);

    public static Specification<Product> InCategory(string categoryName) =>
        Specification.Create<Product>(p => p.Category == categoryName);
}

// Usage
var specification = ProductSpecification
    .IsInStock
    .And(ProductSpecification.HasMinimumQuantity(10))
    .And(ProductSpecification.InPriceRange(10.0m, 100.0m));
```

### Specification Chains

Build complex specifications by chaining multiple conditions:

```csharp
public static class OrderSpecification
{
    public static Specification<Order> IsPending { get; } =
        Specification.Create<Order>(o => o.Status == OrderStatus.Pending);

    public static Specification<Order> IsOverdue(DateTimeOffset now) =>
        Specification.Create<Order>(o => o.DueDate < now);

    public static Specification<Order> HasValue(decimal minimumValue) =>
        Specification.Create<Order>(o => o.TotalAmount >= minimumValue);

    // Complex specification combining multiple conditions
    public static Specification<Order> RequiresAttention(DateTimeOffset now, decimal threshold) =>
        IsPending
            .And(IsOverdue(now))
            .And(HasValue(threshold));
}
```

### Specification for Validation

Use specifications to validate entities against business rules:

```csharp
public class CreateProductValidator
{
    private static readonly Specification<Product> HasValidName =
        Specification.Create<Product>(p => !string.IsNullOrWhiteSpace(p.Name) && p.Name.Length <= 100);

    private static readonly Specification<Product> HasValidPrice =
        Specification.Create<Product>(p => p.Price > 0);

    private static readonly Specification<Product> HasValidQuantity =
        Specification.Create<Product>(p => p.AvailableQuantity >= 0);

    public bool Validate(Product product, out string errorMessage)
    {
        if (!HasValidName.IsSatisfiedBy(product))
        {
            errorMessage = "Product name must be between 1 and 100 characters";
            return false;
        }

        if (!HasValidPrice.IsSatisfiedBy(product))
        {
            errorMessage = "Product price must be greater than zero";
            return false;
        }

        if (!HasValidQuantity.IsSatisfiedBy(product))
        {
            errorMessage = "Product quantity cannot be negative";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}
```

## Testing Specifications

Specifications are highly testable because they encapsulate logic in a reusable way. Here are examples of how to test them:

::: code-group

```csharp [Unit Test - xUnit]
public class ProductSpecificationTests
{
    [Fact]
    public void IsInStock_WhenQuantityIsGreaterThanZero_ShouldReturnTrue()
    {
        // Arrange
        var product = new Product { AvailableQuantity = 5 };
        var specification = ProductSpecification.IsInStock;

        // Act
        bool result = specification.IsSatisfiedBy(product);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsInStock_WhenQuantityIsZero_ShouldReturnFalse()
    {
        // Arrange
        var product = new Product { AvailableQuantity = 0 };
        var specification = ProductSpecification.IsInStock;

        // Act
        bool result = specification.IsSatisfiedBy(product);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(10, 50, 30, true)]
    [InlineData(10, 50, 5, false)]
    [InlineData(10, 50, 60, false)]
    public void InPriceRange_ShouldReturnExpectedResult(
        decimal minPrice, decimal maxPrice, decimal actualPrice, bool expected)
    {
        // Arrange
        var product = new Product { Price = actualPrice };
        var specification = ProductSpecification.InPriceRange(minPrice, maxPrice);

        // Act
        bool result = specification.IsSatisfiedBy(product);

        // Assert
        Assert.Equal(expected, result);
    }
}
```

```csharp [Integration Test]
public class ProductSpecificationIntegrationTests : IDisposable
{
    private readonly IDbQuerySessionFactory _sessionFactory;

    public ProductSpecificationIntegrationTests()
    {
        // Setup test database and session factory
        _sessionFactory = CreateTestSessionFactory();
    }

    [Fact]
    public async Task Query_WithIsInStockSpecification_ShouldReturnOnlyInStockProducts()
    {
        // Arrange
        await using var session = _sessionFactory.Create();
        var specification = ProductSpecification.IsInStock;

        // Act
        var query = session.Query(specification);
        var products = await query.ToListAsync();

        // Assert
        Assert.All(products, p => Assert.True(p.AvailableQuantity > 0));
    }

    public void Dispose()
    {
        // Cleanup test database
    }
}
```

```csharp [Combined Specifications Test]
public class CombinedSpecificationTests
{
    [Fact]
    public void AndSpecification_WhenBothSatisfied_ShouldReturnTrue()
    {
        // Arrange
        var product = new Product
        {
            AvailableQuantity = 10,
            Price = 25.0m
        };

        var specification = ProductSpecification.IsInStock
            .And(ProductSpecification.InPriceRange(10.0m, 50.0m));

        // Act
        bool result = specification.IsSatisfiedBy(product);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void NotSpecification_ShouldInvertResult()
    {
        // Arrange
        var product = new Product { AvailableQuantity = 0 };
        var specification = ProductSpecification.IsInStock.Not();

        // Act
        bool result = specification.IsSatisfiedBy(product);

        // Assert
        Assert.True(result);
    }
}
```

:::

## Error Handling and Edge Cases

When working with specifications, consider these common scenarios:

### Null Safety

```csharp
public static class ProductSpecification
{
    // Handle null strings safely
    public static Specification<Product> HasCategory(string categoryName) =>
        Specification.Create<Product>(p =>
            p.Category != null && p.Category.Equals(categoryName, StringComparison.OrdinalIgnoreCase));

    // Handle null navigation properties
    public static Specification<Product> HasSupplier =>
        Specification.Create<Product>(p => p.Supplier != null);

    // Combine for safe navigation
    public static Specification<Product> FromSupplierInCountry(string country) =>
        HasSupplier.And(Specification.Create<Product>(
            p => p.Supplier!.Country.Equals(country, StringComparison.OrdinalIgnoreCase)));
}
```

### Database Translation Limitations

Some C# expressions cannot be translated to SQL. Keep specifications simple and database-friendly:

::: code-group

```csharp [❌ Bad - Won't Translate]
// Using complex string methods that may not translate to SQL
public static Specification<Product> HasComplexNamePattern { get; } =
    Specification.Create<Product>(p =>
        p.Name.Split(',').Any(part => part.Trim().StartsWith("Pro")));
```

```csharp [✅ Good - Translates Well]
// Using simple, translatable operations
public static Specification<Product> NameStartsWithPro { get; } =
    Specification.Create<Product>(p => p.Name.StartsWith("Pro"));

public static Specification<Product> NameContainsPro { get; } =
    Specification.Create<Product>(p => p.Name.Contains("Pro"));
```

:::

### Exception Handling

```csharp
public class ProductService
{
    private readonly IDbQuerySession _session;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IDbQuerySession session, ILogger<ProductService> logger)
    {
        _session = session;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<Product>>> GetProductsAsync(
        Specification<Product> specification,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _session.Query(specification);
            var products = await query.ToListAsync(cancellationToken);
            return Result<IReadOnlyList<Product>>.Success(products);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Failed to translate specification to SQL");
            return Result<IReadOnlyList<Product>>.Failure("Query translation failed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error querying products");
            return Result<IReadOnlyList<Product>>.Failure("Query execution failed");
        }
    }
}
```