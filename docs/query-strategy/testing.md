# Testing Query Strategies

Query strategies should be tested to ensure they produce correct results. This guide covers both unit testing and integration testing approaches.

## Unit Testing with In-Memory Collections

Unit tests validate the query logic without requiring a database. They use in-memory collections to verify filtering, sorting, and projections:

```csharp
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

## Integration Testing with Database

Integration tests verify that query strategies work correctly with an actual database provider:

```csharp
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

## Testing Dynamic Query Strategies

When testing query strategies with parameters, verify all parameter combinations:

```csharp
public class SearchProductsQueryStrategyTests
{
    [Theory]
    [InlineData("Apple", null, null, 1)]
    [InlineData(null, 10.0, null, 2)]
    [InlineData(null, null, 50.0, 2)]
    [InlineData("Product", 20.0, 100.0, 1)]
    public void Execute_WithDifferentFilters_ShouldReturnExpectedCount(
        string? nameFilter,
        decimal? minPrice,
        decimal? maxPrice,
        int expectedCount)
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = 1, Name = "Apple", Price = 15.0m },
            new() { Id = 2, Name = "Product A", Price = 25.0m },
            new() { Id = 3, Name = "Product B", Price = 75.0m }
        }.AsQueryable();

        var strategy = new SearchProductsQueryStrategy(nameFilter, minPrice, maxPrice);

        // Act
        var result = strategy.Execute(products).ToList();

        // Assert
        Assert.Equal(expectedCount, result.Count);
    }

    [Fact]
    public void Constructor_WithInvalidPriceRange_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new SearchProductsQueryStrategy(minPrice: 100.0m, maxPrice: 50.0m));
    }
}
```

## Testing Value Type Queries

Test query strategies that return value types to ensure nullable semantics work correctly:

```csharp
public class AggregationQueryStrategyTests
{
    [Fact]
    public async Task QueryValue_WithResults_ShouldReturnValue()
    {
        // Arrange
        await using var session = _sessionFactory.Create();
        var strategy = ProductQueryStrategy.GetAveragePrice();

        // Act
        var query = session.QueryValue(strategy);
        var result = await query.FirstOrDefaultAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Value > 0);
    }

    [Fact]
    public async Task QueryValue_WithNoResults_ShouldReturnNull()
    {
        // Arrange
        await using var session = _sessionFactory.Create();
        // Ensure no products exist
        var strategy = ProductQueryStrategy.GetAveragePrice();

        // Act
        var query = session.QueryValue(strategy);
        var result = await query.FirstOrDefaultAsync();

        // Assert
        Assert.Null(result);
    }
}
```

## Best Practices for Testing

### 1. Test the Query Logic, Not the Framework

Focus on testing your business logic (filters, sorting, projections) rather than testing LINQ or Entity Framework functionality.

### 2. Use Realistic Test Data

Create test data that represents real-world scenarios, including edge cases:

```csharp
private static IQueryable<Product> CreateTestProducts()
{
    return new List<Product>
    {
        // Normal cases
        new() { Id = 1, Name = "Product A", Price = 50.0m, AvailableQuantity = 10 },

        // Edge cases
        new() { Id = 2, Name = "", Price = 0.0m, AvailableQuantity = 0 },  // Empty values
        new() { Id = 3, Name = "Product C", Price = decimal.MaxValue, AvailableQuantity = int.MaxValue },  // Max values

        // Boundary cases
        new() { Id = 4, Name = "Product D", Price = 0.01m, AvailableQuantity = 1 }  // Minimum valid values
    }.AsQueryable();
}
```

### 3. Separate Unit and Integration Tests

Keep unit tests fast and isolated, while integration tests verify actual database behavior:

```csharp
// Unit test - fast, no database
[Trait("Category", "Unit")]
public class GetProductsQueryStrategy_UnitTests { }

// Integration test - slower, uses database
[Trait("Category", "Integration")]
public class GetProductsQueryStrategy_IntegrationTests { }
```

### 4. Test Error Conditions

Verify that query strategies handle invalid inputs gracefully:

```csharp
[Fact]
public void Constructor_WithNegativePrice_ShouldThrowException()
{
    Assert.Throws<ArgumentException>(() =>
        new SearchProductsQueryStrategy(minPrice: -10.0m));
}
```