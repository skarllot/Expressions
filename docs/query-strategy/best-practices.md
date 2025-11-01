# Best Practices

This guide covers best practices for error handling and performance optimization when working with query strategies.

## Error Handling

Handle errors gracefully when executing query strategies to provide a better user experience and make debugging easier.

### Validation Before Execution

Validate query strategy parameters in the constructor to fail fast:

```csharp
public class SearchProductsQueryStrategy : EntityQueryStrategy<Product, ProductDto>
{
    private readonly string? _nameFilter;
    private readonly decimal? _minPrice;
    private readonly decimal? _maxPrice;

    public SearchProductsQueryStrategy(
        string? nameFilter = null,
        decimal? minPrice = null,
        decimal? maxPrice = null)
    {
        // Validate parameters
        if (minPrice.HasValue && minPrice.Value < 0)
        {
            throw new ArgumentException("Minimum price cannot be negative", nameof(minPrice));
        }

        if (maxPrice.HasValue && maxPrice.Value < 0)
        {
            throw new ArgumentException("Maximum price cannot be negative", nameof(maxPrice));
        }

        if (minPrice.HasValue && maxPrice.HasValue && minPrice.Value > maxPrice.Value)
        {
            throw new ArgumentException("Minimum price cannot be greater than maximum price");
        }

        _nameFilter = nameFilter;
        _minPrice = minPrice;
        _maxPrice = maxPrice;
    }

    protected override IQueryable<ProductDto> ExecuteCore(IQueryable<Product> source)
    {
        var query = source.AsQueryable();

        if (!string.IsNullOrWhiteSpace(_nameFilter))
        {
            query = query.Where(p => p.Name.Contains(_nameFilter));
        }

        if (_minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= _minPrice.Value);
        }

        if (_maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= _maxPrice.Value);
        }

        return query.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price
        });
    }
}
```

## Performance Considerations

Follow these guidelines to ensure your query strategies perform efficiently.

### Avoid N+1 Queries

When working with related entities, be mindful of the N+1 query problem:

::: code-group

```csharp [❌ Bad - N+1 Problem]
public class GetOrdersWithCustomersQueryStrategy : EntityQueryStrategy<Order, Order>
{
    protected override IQueryable<OrderDto> ExecuteCore(IQueryable<Order> source)
    {
        // This may cause N+1 queries if Customer is not loaded
        return source;
    }
}
```

```csharp [✅ Good - Projection]
public class GetOrdersWithCustomersQueryStrategy : EntityQueryStrategy<Order, OrderDto>
{
    protected override IQueryable<OrderDto> ExecuteCore(IQueryable<Order> source)
    {
        // Projection in Select avoids N+1 by translating to SQL JOIN
        return source.Select(o => new OrderDto
        {
            Id = o.Id,
            CustomerName = o.Customer.Name
        });
    }
}
```

:::

### Use Projections for Performance

Use projections to select only the data you need:

```csharp
// Instead of returning entire entities
public class GetProductDetailsQueryStrategy : EntityQueryStrategy<Product, ProductDetailDto>
{
    protected override IQueryable<ProductDetailDto> ExecuteCore(IQueryable<Product> source)
    {
        // Project to DTO with only required fields
        return source.Select(p => new ProductDetailDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            CategoryName = p.Category
            // Don't include heavy fields like images, descriptions unless needed
        });
    }
}
```

### Limit Result Sets

Always consider limiting result sets for better performance:

```csharp
public static class ProductQueryStrategy
{
    public static IEntityQueryStrategy<Product, ProductDto> GetTopSellingProducts(int count = 10) =>
        QueryStrategy.CreateForEntity(
            (IQueryable<Product> source) => source
                .Where(ProductSpecification.IsInStock)
                .OrderByDescending(p => p.SalesCount)
                .Take(count)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    SalesCount = p.SalesCount
                }));
}
```

### Optimize Filtering

Place the most restrictive filters first to reduce the dataset early:

```csharp
public class GetRecentOrdersQueryStrategy : EntityQueryStrategy<Order, OrderDto>
{
    protected override IQueryable<OrderDto> ExecuteCore(IQueryable<Order> source)
    {
        // Apply most restrictive filter first
        return source
            .Where(o => o.CreatedAt >= DateTime.UtcNow.AddDays(-7))  // Most restrictive
            .Where(o => o.Status == OrderStatus.Completed)           // Less restrictive
            .Where(o => o.TotalAmount > 0)                            // Least restrictive
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                TotalAmount = o.TotalAmount,
                CreatedAt = o.CreatedAt
            });
    }
}
```

### Avoid Client-Side Evaluation

Ensure operations can be translated to SQL to avoid client-side evaluation:

::: code-group

```csharp [❌ Bad - Client-Side Evaluation]
public class GetProductsQueryStrategy : EntityQueryStrategy<Product, ProductDto>
{
    protected override IQueryable<ProductDto> ExecuteCore(IQueryable<Product> source)
    {
        // Complex method call may not translate to SQL
        return source.Select(p => new ProductDto
        {
            Id = p.Id,
            DisplayName = FormatProductName(p.Name)  // Client-side evaluation!
        });
    }

    private string FormatProductName(string name) => /* complex logic */;
}
```

```csharp [✅ Good - Database-Side Operations]
public class GetProductsQueryStrategy : EntityQueryStrategy<Product, ProductDto>
{
    protected override IQueryable<ProductDto> ExecuteCore(IQueryable<Product> source)
    {
        // Simple operations that translate to SQL
        return source.Select(p => new ProductDto
        {
            Id = p.Id,
            DisplayName = p.Name.ToUpper()  // Translates to SQL UPPER()
        });
    }
}
```

:::

### Use Appropriate Indexes

Work with your database team to ensure proper indexes exist for commonly filtered and sorted columns:

```csharp
// If this query is slow, ensure indexes exist on:
// - Product.Category
// - Product.Price
// - Product.AvailableQuantity
public class GetProductsByCategoryQueryStrategy : EntityQueryStrategy<Product, ProductDto>
{
    private readonly string _category;

    public GetProductsByCategoryQueryStrategy(string category)
    {
        _category = category;
    }

    protected override IQueryable<ProductDto> ExecuteCore(IQueryable<Product> source)
    {
        return source
            .Where(p => p.Category == _category)           // Index on Category
            .Where(p => p.AvailableQuantity > 0)           // Index on AvailableQuantity
            .OrderBy(p => p.Price)                         // Index on Price
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
            });
    }
}
```

## General Guidelines

### 1. Keep Query Strategies Focused

Each query strategy should have a single, well-defined purpose:

```csharp
// Good - Single purpose
public class GetActiveProductsQueryStrategy : EntityQueryStrategy<Product, ProductDto> { }
public class GetDiscontinuedProductsQueryStrategy : EntityQueryStrategy<Product, ProductDto> { }

// Avoid - Multiple purposes
public class GetProductsQueryStrategy : EntityQueryStrategy<Product, ProductDto>
{
    public GetProductsQueryStrategy(bool activeOnly, bool sortByPrice, bool includeOutOfStock)
    {
        // Too many responsibilities
    }
}
```

### 2. Make Query Strategies Immutable

Query strategy instances should be immutable after construction:

```csharp
public class GetProductsQueryStrategy : EntityQueryStrategy<Product, ProductDto>
{
    // No setters - immutable
    public required decimal MinPrice { get; init; } // Set when initializing only
}
```

### 3. Document Complex Queries

Add XML documentation to complex query strategies:

```csharp
/// <summary>
/// Retrieves products that have been restocked within the specified number of days
/// and have a minimum quantity available. Results are ordered by restock date.
/// </summary>
/// <remarks>
/// This query joins Products with InventoryLogs to find recent restocking events.
/// Performance note: Ensure indexes exist on InventoryLogs.RestockDate and Product.AvailableQuantity.
/// </remarks>
public class GetRecentlyRestockedProductsQueryStrategy : EntityQueryStrategy<Product, ProductDto>
{
    // Implementation
}
```
