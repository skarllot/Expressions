# Usage Patterns

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
var specification = ProductSpecification.IsInStock
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
