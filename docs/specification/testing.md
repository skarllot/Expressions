# Testing Specifications

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