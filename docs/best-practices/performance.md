# Performance Best Practices

Optimizing the performance of applications using Raiqub Expressions requires understanding how specifications and query strategies translate to database queries and how to leverage database capabilities effectively.

## Overview

Key areas for optimization:

- **Query Optimization** - Write efficient specifications and query strategies
- **Database Indexing** - Ensure proper indexes on queried columns
- **Projection** - Select only needed data
- **Caching** - Cache frequently accessed data
- **Pagination** - Handle large result sets efficiently
- **Monitoring** - Profile and measure query performance

## Query Optimization

### Use Projections

Always project to DTOs rather than returning full entities:

::: code-group

```csharp [❌ Bad - Full Entity]
public class GetProductsQueryStrategy : EntityQueryStrategy<Product, Product>
{
    protected override IQueryable<Product> ExecuteCore(IQueryable<Product> source)
    {
        return source
            .Where(ProductSpecification.IsInStock)
            .OrderBy(p => p.Name);
        // Returns entire Product entity with all columns
    }
}
```

```csharp [✅ Good - Projection]
public class GetProductsQueryStrategy : EntityQueryStrategy<Product, ProductDto>
{
    protected override IQueryable<ProductDto> ExecuteCore(IQueryable<Product> source)
    {
        return source
            .Where(ProductSpecification.IsInStock)
            .OrderBy(p => p.Name)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
                // Only select needed columns
            });
    }
}
```

:::

**Why?** Projections reduce:
- Network bandwidth usage
- Memory consumption
- Serialization overhead
- Data transferred from database

### Avoid N+1 Queries

Use projections with navigation properties to avoid N+1 query problems:

::: code-group

```csharp [❌ Bad - Potential N+1]
public class GetOrdersQueryStrategy : EntityQueryStrategy<Order, Order>
{
    protected override IQueryable<Order> ExecuteCore(IQueryable<Order> source)
    {
        // This may trigger N+1 queries when accessing Customer property
        return source.Where(o => o.Status == OrderStatus.Completed);
    }
}

// Usage that causes N+1:
var orders = await query.ToListAsync();
foreach (var order in orders)
{
    Console.WriteLine(order.Customer.Name); // Separate query per order!
}
```

```csharp [✅ Good - Single Query with Projection]
public class GetOrdersQueryStrategy : EntityQueryStrategy<Order, OrderDto>
{
    protected override IQueryable<OrderDto> ExecuteCore(IQueryable<Order> source)
    {
        return source
            .Where(o => o.Status == OrderStatus.Completed)
            .Select(o => new OrderDto
            {
                OrderId = o.Id,
                CustomerName = o.Customer.Name, // Translated to JOIN
                TotalAmount = o.TotalAmount
            });
    }
}
```

:::

### Keep Specifications Simple

Complex operations may not translate to SQL efficiently:

::: code-group

```csharp [❌ Bad - Complex Logic]
public static class ProductSpecification
{
    public static Specification<Product> HasComplexCalculation { get; } =
        Specification.Create<Product>(p =>
            // Complex calculations that may not translate
            p.Name.Split(' ').Length > 2 &&
            DateTime.Parse(p.CreatedAt.ToString()).Year == 2024);
}
```

```csharp [✅ Good - Simple Translatable Logic]
public static class ProductSpecification
{
    public static Specification<Product> HasLongName { get; } =
        Specification.Create<Product>(p => p.Name.Length > 10);

    public static Specification<Product> CreatedInYear(int year) =>
        Specification.Create<Product>(p => p.CreatedAt.Year == year);
}
```

:::

### Use Parameterized Queries

EF Core automatically parameterizes queries to enable query plan caching:

```csharp
// Good - Value is automatically parameterized
public static Specification<Product> InCategory(string category) =>
    Specification.Create<Product>(p => p.Category == category);

// Usage
var spec = ProductSpecification.InCategory("Electronics"); // Parameter: @p0 = 'Electronics'
```

### Limit Result Sets

Always consider limiting results for list queries:

```csharp
public static class ProductQueryStrategy
{
    // For lists, use pagination or Take()
    public static IEntityQueryStrategy<Product, ProductDto> GetTopProducts(int count = 20) =>
        QueryStrategy.CreateForEntity(
            (IQueryable<Product> source) => source
                .OrderByDescending(p => p.Sales)
                .Take(count) // Translates to TOP/LIMIT in SQL
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Sales = p.Sales
                }));
}
```

## Database Indexing

### Index Specification Columns

Create indexes on columns used in specifications:

```csharp
// Specification
public static Specification<Product> IsInStock { get; } =
    Specification.Create<Product>(p => p.AvailableQuantity > 0);

public static Specification<Product> InCategory(string category) =>
    Specification.Create<Product>(p => p.Category == category);
```

```sql
-- Create indexes for these specifications
CREATE INDEX IX_Products_AvailableQuantity ON Products(AvailableQuantity);
CREATE INDEX IX_Products_Category ON Products(Category);
```

### Composite Indexes

For combined specifications, consider composite indexes:

```csharp
// Combined specification
var spec = ProductSpecification.IsInStock
    .And(ProductSpecification.InCategory("Electronics"))
    .And(ProductSpecification.InPriceRange(100, 500));
```

```sql
-- Composite index for this query pattern
CREATE INDEX IX_Products_Category_Price_Quantity
ON Products(Category, Price, AvailableQuantity)
WHERE AvailableQuantity > 0;
```

### Configure Indexes in EF Core

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Product>(entity =>
    {
        // Single column index
        entity.HasIndex(p => p.AvailableQuantity)
            .HasDatabaseName("IX_Products_AvailableQuantity");

        // Composite index
        entity.HasIndex(p => new { p.Category, p.Price })
            .HasDatabaseName("IX_Products_Category_Price");

        // Filtered index (SQL Server, PostgreSQL)
        entity.HasIndex(p => p.Category)
            .HasDatabaseName("IX_Products_ActiveCategory")
            .HasFilter("IsActive = 1");
    });
}
```

## Pagination Strategies

### Use Built-in Pagination

Raiqub Expressions provides efficient pagination through `ToPagedListAsync`:

```csharp
public async Task<PagedResult<ProductDto>> GetProductsAsync(
    int pageNumber,
    int pageSize,
    CancellationToken cancellationToken = default)
{
    var query = _session.Query(new GetProductsQueryStrategy());

    // Efficient pagination with single query for data + count
    return await query.ToPagedListAsync(pageNumber, pageSize, cancellationToken);
}
```

**How it works:**
- Single roundtrip to database
- Efficient `OFFSET`/`FETCH` or `SKIP`/`TAKE` in SQL
- Total count retrieved with `COUNT(*) OVER()`

### Keyset Pagination for Large Datasets

For very large datasets, consider keyset (seek) pagination:

```csharp
public class GetProductsAfterQueryStrategy : EntityQueryStrategy<Product, ProductDto>
{
    private readonly Guid _afterId;
    private readonly int _pageSize;

    public GetProductsAfterQueryStrategy(Guid afterId, int pageSize = 20)
    {
        _afterId = afterId;
        _pageSize = pageSize;
    }

    protected override IQueryable<ProductDto> ExecuteCore(IQueryable<Product> source)
    {
        return source
            .Where(p => p.Id > _afterId) // Keyset condition
            .OrderBy(p => p.Id)
            .Take(_pageSize)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
            });
    }
}
```

**Benefits:**
- Consistent performance regardless of page number
- No OFFSET overhead
- Works well with infinite scrolling

## Caching Strategies

### Query Result Caching

Cache frequently accessed, rarely changing data:

```csharp
public class CachedProductService
{
    private readonly IDbQuerySession _session;
    private readonly IMemoryCache _cache;

    public CachedProductService(IDbQuerySession session, IMemoryCache cache)
    {
        _session = session;
        _cache = cache;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(
        CancellationToken cancellationToken = default)
    {
        const string cacheKey = "product_categories";

        // Try to get from cache
        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<CategoryDto> categories))
        {
            return categories;
        }

        // Query database
        var query = _session.Query(new GetCategoriesQueryStrategy());
        categories = await query.ToListAsync(cancellationToken);

        // Cache for 1 hour
        _cache.Set(cacheKey, categories, TimeSpan.FromHours(1));

        return categories;
    }
}
```

### Specification Result Caching

Cache specification evaluation results for in-memory collections:

```csharp
public class CachedSpecificationEvaluator<T>
{
    private readonly ConcurrentDictionary<string, Func<T, bool>> _cache = new();

    public bool Evaluate(T entity, Specification<T> specification)
    {
        var key = specification.GetType().FullName;

        var compiled = _cache.GetOrAdd(key, _ =>
            specification.ToExpression().Compile());

        return compiled(entity);
    }
}
```

### Distributed Caching

Use distributed cache for multi-instance deployments:

```csharp
public class DistributedCacheProductService
{
    private readonly IDbQuerySession _session;
    private readonly IDistributedCache _cache;

    public DistributedCacheProductService(
        IDbQuerySession session,
        IDistributedCache cache)
    {
        _session = session;
        _cache = cache;
    }

    public async Task<ProductDto?> GetProductAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"product:{id}";

        // Try to get from distributed cache
        var cached = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (cached != null)
        {
            return JsonSerializer.Deserialize<ProductDto>(cached);
        }

        // Query database
        var query = _session.Query<Product>().Where(p => p.Id == id);
        var product = await query
            .Select(p => new ProductDto { Id = p.Id, Name = p.Name, Price = p.Price })
            .FirstOrDefaultAsync(cancellationToken);

        if (product != null)
        {
            // Cache for 5 minutes
            var serialized = JsonSerializer.Serialize(product);
            await _cache.SetStringAsync(
                cacheKey,
                serialized,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                },
                cancellationToken);
        }

        return product;
    }
}
```

## Async Streaming

For large result sets, use async streaming to reduce memory usage:

```csharp
public async Task ProcessLargeDatasetAsync(CancellationToken cancellationToken = default)
{
    var query = _session.Query(new GetAllProductsQueryStrategy());

    // Stream results instead of loading all into memory
    await foreach (var product in query.ToAsyncEnumerable(cancellationToken))
    {
        await ProcessProductAsync(product, cancellationToken);

        // Memory is released as each product is processed
    }
}
```

**Benefits:**
- Lower memory footprint
- Start processing immediately
- Handle datasets larger than available memory

## Entity Framework Specific Optimizations

### Query Splitting

For queries with multiple collections, consider split queries to avoid cartesian explosion:

```csharp
services.AddEntityFrameworkExpressions()
    .Configure<Order>(options => options.UseSplitQuery = true)
    .AddSingleContext<AppDbContext>();
```

Or per query:

```csharp
public class GetOrderWithDetailsQueryStrategy : EntityQueryStrategy<Order, OrderDto>
{
    protected override IQueryable<OrderDto> ExecuteCore(IQueryable<Order> source)
    {
        return source
            .AsSplitQuery() // Force split query for this query only
            .Select(o => new OrderDto
            {
                Id = o.Id,
                Items = o.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            });
    }
}
```

### Compiled Queries

For frequently executed queries, use compiled queries:

```csharp
private static readonly Func<AppDbContext, Guid, Task<Product?>> GetProductByIdCompiled =
    EF.CompileAsyncQuery((AppDbContext context, Guid id) =>
        context.Products.FirstOrDefault(p => p.Id == id));

public async Task<Product?> GetProductAsync(Guid id)
{
    await using var context = _contextFactory.CreateDbContext();
    return await GetProductByIdCompiled(context, id);
}
```

### No Tracking for Read-Only Queries

Disable change tracking for read-only operations:

```csharp
// Set globally for all sessions
services.AddEntityFrameworkExpressions()
    .AddSingleContext<AppDbContext>(ChangeTracking.Disabled);

// Or per query
public class GetProductsQueryStrategy : EntityQueryStrategy<Product, ProductDto>
{
    protected override IQueryable<ProductDto> ExecuteCore(IQueryable<Product> source)
    {
        return source
            .AsNoTracking() // Disable tracking for this query
            .Where(ProductSpecification.IsInStock)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
            });
    }
}
```

## Monitoring and Profiling

### Log Generated SQL

Enable logging to review generated SQL queries:

```csharp
services.AddDbContextFactory<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString)
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging() // Only in development!
        .EnableDetailedErrors();
});
```

### Use ToQueryString for Testing

Verify SQL generation in tests:

```csharp
[Fact]
public void QueryStrategy_ShouldGenerateEfficientSql()
{
    // Arrange
    var strategy = new GetProductsQueryStrategy();
    var source = _context.Set<Product>();

    // Act
    var query = strategy.Execute(source);
    var sql = query.ToQueryString();

    // Assert
    Assert.DoesNotContain("SELECT *", sql); // Should use projection
    Assert.Contains("WHERE", sql);
    _testOutputHelper.WriteLine(sql);
}
```

### Application Insights Integration

Track query performance with Application Insights:

```csharp
public class MonitoredProductService
{
    private readonly IDbQuerySession _session;
    private readonly TelemetryClient _telemetry;

    public MonitoredProductService(
        IDbQuerySession _session,
        TelemetryClient telemetry)
    {
        _session = session;
        _telemetry = telemetry;
    }

    public async Task<IReadOnlyList<ProductDto>> GetProductsAsync(
        CancellationToken cancellationToken = default)
    {
        using var operation = _telemetry.StartOperation<DependencyTelemetry>("GetProducts");

        try
        {
            var query = _session.Query(new GetProductsQueryStrategy());
            var results = await query.ToListAsync(cancellationToken);

            operation.Telemetry.ResultCode = "Success";
            operation.Telemetry.Properties["ResultCount"] = results.Count.ToString();

            return results;
        }
        catch (Exception ex)
        {
            operation.Telemetry.Success = false;
            _telemetry.TrackException(ex);
            throw;
        }
    }
}
```

## Connection Management

### Connection Pooling

Ensure connection pooling is enabled (default in most providers):

```csharp
// Entity Framework Core - Connection pooling is automatic
services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(connectionString)); // Pooling enabled by default

// Marten - Configure connection pooling
services.AddMarten(options =>
{
    options.Connection(connectionString);
    // Connection pooling is handled by Npgsql
});
```

### Session Lifetime

Use short-lived sessions and dispose properly:

```csharp
// Good - Short-lived session
public async Task<ProductDto> GetProductAsync(Guid id)
{
    await using var session = _sessionFactory.Create();
    var query = session.Query<Product>().Where(p => p.Id == id);
    return await query
        .Select(p => new ProductDto { Id = p.Id, Name = p.Name })
        .FirstAsync();
}

// Bad - Long-lived session
private readonly IDbSession _session; // Don't inject as singleton!
```

## Performance Checklist

### Query Design

- [ ] Use projections to select only needed columns
- [ ] Avoid N+1 queries with proper projections
- [ ] Keep specifications simple and translatable
- [ ] Limit result sets with `Take()` or pagination
- [ ] Use `AsNoTracking()` for read-only queries

### Database

- [ ] Create indexes on commonly queried columns
- [ ] Use composite indexes for combined filters
- [ ] Monitor index usage and fragmentation
- [ ] Review query execution plans
- [ ] Consider partitioning for very large tables

### Caching

- [ ] Cache frequently accessed, rarely changing data
- [ ] Use appropriate cache durations
- [ ] Implement cache invalidation strategies
- [ ] Consider distributed caching for scalability

### Monitoring

- [ ] Log slow queries
- [ ] Track query execution times
- [ ] Monitor connection pool usage
- [ ] Review SQL query plans
- [ ] Profile memory usage

## Common Performance Anti-Patterns

### ❌ Loading Full Entities

```csharp
// Bad - Loads all columns
var products = await _session.Query<Product>().ToListAsync();
```

### ❌ Client-Side Evaluation

```csharp
// Bad - Filtering happens in memory after loading all data
var filtered = (await _session.Query<Product>().ToListAsync())
    .Where(p => p.IsInStock())
    .ToList();
```

### ❌ Multiple Queries in Loop

```csharp
// Bad - N+1 queries
foreach (var orderId in orderIds)
{
    var order = await _session.Query<Order>()
        .Where(o => o.Id == orderId)
        .FirstAsync();
}
```

### ❌ Unbounded Result Sets

```csharp
// Bad - Could return millions of rows
var allProducts = await _session.Query<Product>().ToListAsync();
```

## See Also

- [Query Strategy](/query-strategy/) - Writing efficient query strategies
- [Specification](/specification/) - Creating performant specifications
- [Entity Framework Core](/ef-core/) - EF Core specific optimizations
- [Testing](testing) - Testing query performance
- [EF Core Performance](https://learn.microsoft.com/en-us/ef/core/performance/) - Official EF Core performance guide
