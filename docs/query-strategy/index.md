# Query Strategy

The query strategy is based on the Strategy Pattern by defining a strategy for querying the database allowing better concern separation, maintainability and reusability than the repository pattern.

The **\`Raiqub.Expressions.Reading\`** package provides abstractions for creating query strategies. You can create a new query strategy by choosing one of several ways available to implement a query strategy.

To add Raiqub.Expressions.Reading library to a .NET project, go get it from Nuget:

::: code-group

```shell [.NET CLI]
dotnet add package Raiqub.Expressions.Reading
```

```powershell [Powershell]
PM> Install-Package Raiqub.Expressions.Reading
```

```shell [Paket]
paket add nuget Raiqub.Expressions.Reading
```

:::

## Single Entity Query

The most common strategy is querying a single entity and for that purpose the interface `IEntityQueryStrategy<TSource, TResult>` and its abstract class implementation `EntityQueryStrategy<TSource, TResult>` was created.

Here's an example of a entity query strategy that filters a list of entities based on a set of conditions:

::: code-group

```csharp [Method Chain]
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

```csharp [Query Syntax]
public class GetProductNameQueryStrategy : EntityQueryStrategy<Product, ProductName>
{
    protected override IQueryable<ProductName> ExecuteCore(IQueryable<Product> source)
    {
        return from p in source.Where(ProductSpecification.IsInStock)
            orderby p.Name
            select new ProductName { Id = p.Id, Name = p.Name };
    }
}
```

:::

or, you can define only the preconditions:

::: code-group

```csharp [Named Specification]
public class GetProductInStockQueryStrategy : EntityQueryStrategy<Product>
{
    protected override IEnumerable<Specification<Product>> GetPreconditions()
    {
        yield new ProductIsInStock();
    }
}
```

```csharp [Anonymous Specification]
public class GetProductInStockQueryStrategy : EntityQueryStrategy<Product>
{
    protected override IEnumerable<Specification<Product>> GetPreconditions()
    {
        yield Specification.Create<Product>(p => p.AvailableQuantity > 0);
    }
}
```

:::

or yet, you can create a static class as a provider of query strategies:

::: code-group

```csharp [Method Chain]
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

```csharp [Query Syntax]
public static class ProductQueryStrategy
{
    public static IEntityQueryStrategy<Product, ProductName> GetName() =>
        QueryStrategy.CreateForEntity(
            (IQueryable<Product> source) =>
                from p in source.Where(ProductSpecification.IsInStock)
                orderby p.Name
                select new ProductName { Id = p.Id, Name = p.Name });
}
```

:::

## Multiple Entities Query

For the cases where multiple entities need to be queried, the interface `IQueryStrategy<TResult>` was created.

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

## Query Strategies Returning Value Types

When your query strategy returns value types (structs) such as `int`, `decimal`, `DateTime`, or other structs, you should use them with the `QueryValue()` method instead of `Query()`. Both `IEntityQueryStrategy<TSource, TResult>` and `IQueryStrategy<TResult>` support value type results when `TResult` is constrained to `struct`.

::: code-group

```csharp [Entity Query - Class]
public class GetActiveProductCountQueryStrategy : EntityQueryStrategy<Product, int>
{
    protected override IQueryable<int> ExecuteCore(IQueryable<Product> source)
    {
        return source
            .Where(ProductSpecification.IsInStock)
            .Select(p => p.AvailableQuantity);
    }
}
```

```csharp [Entity Query - Factory]
public static class ProductQueryStrategy
{
    public static IEntityQueryStrategy<Product, decimal> GetAveragePrice() =>
        QueryStrategy.CreateForEntity(
            (IQueryable<Product> source) => source
                .Where(ProductSpecification.IsInStock)
                .Select(p => p.Price));
}
```

```csharp [Multi-Entity Query]
public static class OrderQueryStrategy
{
    public static IQueryStrategy<decimal> GetTotalRevenue() =>
        QueryStrategy.Create(
            source => from order in source.GetSet<Order>()
                where order.Status == OrderStatus.Completed
                select order.TotalAmount);
}
```

:::

These query strategies should be used with the `QueryValue()` method on `IDbQuerySession`:

```csharp
await using (var session = querySessionFactory.Create())
{
    // Using entity query strategy
    IDbQueryValue<int> quantityQuery = session.QueryValue(new GetActiveProductCountQueryStrategy());
    int? totalQuantity = await quantityQuery.FirstOrDefaultAsync();

    // Using factory method
    IDbQueryValue<decimal> priceQuery = session.QueryValue(ProductQueryStrategy.GetAveragePrice());
    decimal? avgPrice = await priceQuery.FirstOrDefaultAsync();

    // Using multi-entity query strategy
    IDbQueryValue<decimal> revenueQuery = session.QueryValue(OrderQueryStrategy.GetTotalRevenue());
    decimal? revenue = await revenueQuery.SingleOrDefaultAsync();
}
```

**Why use QueryValue for value types?**

The `QueryValue()` method returns `IDbQueryValue<T>` which provides proper nullable semantics for value types. Methods like `FirstOrDefaultAsync()` and `SingleOrDefaultAsync()` return `T?` (nullable value type), allowing you to distinguish between "no result found" (null) and a result with a zero value.

## Common Query Patterns

### Pagination

The `IDbQueryBase<T>` interface provides built-in `ToPagedListAsync` methods for pagination. You don't need to implement Skip/Take in your query strategies:

::: code-group

```csharp [Using ToPagedListAsync]
public class ProductService
{
    private readonly IDbQuerySession _session;

    public ProductService(IDbQuerySession session)
    {
        _session = session;
    }

    public async Task<PagedResult<ProductDto>> GetProductsAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _session.Query(new GetProductsQueryStrategy());

        // ToPagedListAsync handles pagination automatically
        return await query.ToPagedListAsync(pageNumber, pageSize, cancellationToken);
    }
}

public class GetProductsQueryStrategy : EntityQueryStrategy<Product, ProductDto>
{
    protected override IQueryable<ProductDto> ExecuteCore(IQueryable<Product> source)
    {
        // Just define the query - no Skip/Take needed
        return source
            .Where(ProductSpecification.IsInStock)
            .OrderBy(p => p.Name)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
            });
    }
}
```

```csharp [Working with PagedResult]
public async Task DisplayProducts(int pageNumber = 1, int pageSize = 10)
{
    var pagedResult = await _productService.GetProductsAsync(pageNumber, pageSize);

    Console.WriteLine($"Page {pagedResult.PageNumber} of {pagedResult.PageCount}");
    Console.WriteLine($"Total items: {pagedResult.TotalCount}");
    Console.WriteLine($"Items on this page: {pagedResult.Count}");

    foreach (var product in pagedResult)
    {
        Console.WriteLine($"{product.Name}: ${product.Price}");
    }

    if (pagedResult.HasNextPage)
    {
        Console.WriteLine("More pages available");
    }
}
```

:::

The `PagedResult<T>` class provides useful properties:
- `PageNumber` - Current page number (one-based)
- `PageSize` - Page size
- `TotalCount` - Total number of records
- `PageCount` - Total number of pages
- `HasPreviousPage` / `HasNextPage` - Navigation helpers
- `IsFirstPage` / `IsLastPage` - Position indicators
- `FirstItemOnPage` / `LastItemOnPage` - Item indices

### Sorting

Create flexible sorting strategies that can be configured at runtime:

```csharp
public class GetSortedProductsQueryStrategy : EntityQueryStrategy<Product, ProductDto>
{
    private readonly string _sortBy;
    private readonly bool _ascending;

    public GetSortedProductsQueryStrategy(string sortBy = "Name", bool ascending = true)
    {
        _sortBy = sortBy;
        _ascending = ascending;
    }

    protected override IQueryable<ProductDto> ExecuteCore(IQueryable<Product> source)
    {
        var query = source.Where(ProductSpecification.IsInStock);

        query = _sortBy switch
        {
            "Name" => _ascending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
            "Price" => _ascending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
            "Quantity" => _ascending ? query.OrderBy(p => p.AvailableQuantity) : query.OrderByDescending(p => p.AvailableQuantity),
            _ => query.OrderBy(p => p.Id)
        };

        return query.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price
        });
    }
}
```

### Dynamic Filtering

Build query strategies that support multiple optional filters:

```csharp
public class SearchProductsQueryStrategy : EntityQueryStrategy<Product, ProductDto>
{
    private readonly string? _nameFilter;
    private readonly decimal? _minPrice;
    private readonly decimal? _maxPrice;
    private readonly string? _category;

    public SearchProductsQueryStrategy(
        string? nameFilter = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? category = null)
    {
        _nameFilter = nameFilter;
        _minPrice = minPrice;
        _maxPrice = maxPrice;
        _category = category;
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

        if (!string.IsNullOrWhiteSpace(_category))
        {
            query = query.Where(p => p.Category == _category);
        }

        return query.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Category = p.Category
        });
    }
}
```

### Aggregation

Use query strategies for aggregation operations:

```csharp
public static class ProductQueryStrategy
{
    public static IEntityQueryStrategy<Product, decimal> GetAveragePriceByCategory(string category) =>
        QueryStrategy.CreateForEntity(
            (IQueryable<Product> source) => source
                .Where(p => p.Category == category && p.AvailableQuantity > 0)
                .Select(p => p.Price));

    public static IQueryStrategy<ProductStatistics> GetStatistics() =>
        QueryStrategy.Create(
            source => from product in source.GetSet<Product>()
                where product.AvailableQuantity > 0
                group product by product.Category into g
                select new ProductStatistics
                {
                    Category = g.Key,
                    TotalProducts = g.Count(),
                    AveragePrice = g.Average(p => p.Price),
                    TotalValue = g.Sum(p => p.Price * p.AvailableQuantity)
                });
}

// Usage
await using var session = sessionFactory.Create();
var statsQuery = session.Query(ProductQueryStrategy.GetStatistics());
var statistics = await statsQuery.ToListAsync();
```

## Testing Query Strategies

Query strategies should be tested to ensure they produce correct results:

::: code-group

```csharp [Unit Test - In-Memory]
public class GetProductsQueryStrategyTests
{
    [Fact]
    public void Execute_ShouldFilterInStockProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = 1, Name = "Product A", AvailableQuantity = 10 },
            new() { Id = 2, Name = "Product B", AvailableQuantity = 0 },
            new() { Id = 3, Name = "Product C", AvailableQuantity = 5 }
        }.AsQueryable();

        var strategy = new GetProductsQueryStrategy();

        // Act
        var result = strategy.Execute(products).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.Name == "Product A");
        Assert.Contains(result, p => p.Name == "Product C");
        Assert.DoesNotContain(result, p => p.Name == "Product B");
    }

    [Fact]
    public void Execute_ShouldOrderByName()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = 1, Name = "Zebra", AvailableQuantity = 10 },
            new() { Id = 2, Name = "Apple", AvailableQuantity = 5 },
            new() { Id = 3, Name = "Mango", AvailableQuantity = 3 }
        }.AsQueryable();

        var strategy = new GetProductsQueryStrategy();

        // Act
        var result = strategy.Execute(products).ToList();

        // Assert
        Assert.Equal("Apple", result[0].Name);
        Assert.Equal("Mango", result[1].Name);
        Assert.Equal("Zebra", result[2].Name);
    }
}
```

```csharp [Integration Test]
public class ProductQueryStrategyIntegrationTests : IAsyncLifetime
{
    private readonly IDbQuerySessionFactory _sessionFactory;

    public ProductQueryStrategyIntegrationTests()
    {
        _sessionFactory = CreateTestSessionFactory();
    }

    public async Task InitializeAsync()
    {
        // Seed test data
        await using var session = _sessionFactory.Create();
        session.Add(new Product { Name = "Test Product", AvailableQuantity = 10, Price = 50.0m });
        await session.SaveChangesAsync();
    }

    [Fact]
    public async Task Query_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        await using var session = _sessionFactory.Create();
        var strategy = new GetProductsQueryStrategy();

        // Act
        var query = session.Query(strategy);
        var pagedResult = await query.ToPagedListAsync(pageNumber: 1, pageSize: 10);

        // Assert
        Assert.NotEmpty(pagedResult);
        Assert.True(pagedResult.Count <= 10);
        Assert.Equal(1, pagedResult.PageNumber);
    }

    public async Task DisposeAsync()
    {
        // Cleanup test database
    }
}
```

:::

## Error Handling

Handle errors gracefully when executing query strategies:

### Database Translation Errors

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

    public async Task<Result<IReadOnlyList<ProductDto>>> GetProductsAsync(
        IEntityQueryStrategy<Product, ProductDto> queryStrategy,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _session.Query(queryStrategy);
            var products = await query.ToListAsync(cancellationToken);
            return Result<IReadOnlyList<ProductDto>>.Success(products);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("could not be translated"))
        {
            _logger.LogError(ex, "Query strategy could not be translated to SQL");
            return Result<IReadOnlyList<ProductDto>>.Failure(
                "The query contains operations that cannot be executed on the database");
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Query was cancelled");
            return Result<IReadOnlyList<ProductDto>>.Failure("Operation was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error executing query strategy");
            return Result<IReadOnlyList<ProductDto>>.Failure("Failed to retrieve products");
        }
    }
}
```

### Query Timeout Handling

```csharp
public class OrderService
{
    private readonly IDbQuerySession _session;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IDbQuerySession session, ILogger<OrderService> logger)
    {
        _session = session;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Order>?> GetOrdersWithTimeoutAsync(
        IEntityQueryStrategy<Order, Order> queryStrategy,
        TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource(timeout);

        try
        {
            var query = _session.Query(queryStrategy);
            return await query.ToListAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Query execution exceeded timeout of {Timeout}", timeout);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing query");
            throw;
        }
    }
}
```

### Validation Before Execution

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

### Avoid N+1 Queries

When working with related entities, be mindful of the N+1 query problem:

::: code-group

```csharp [❌ Bad - N+1 Problem]
public class GetOrdersWithCustomersQueryStrategy : EntityQueryStrategy<Order, OrderDto>
{
    protected override IQueryable<OrderDto> ExecuteCore(IQueryable<Order> source)
    {
        // This may cause N+1 queries if Customer is not loaded
        return source.Select(o => new OrderDto
        {
            Id = o.Id,
            CustomerName = o.Customer.Name  // Potential N+1 issue
        });
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

### Projection for Performance

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