# Common Query Patterns

This guide covers common patterns and techniques when working with query strategies.

## Pagination

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

## Sorting

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

## Dynamic Filtering

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

## Aggregation

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