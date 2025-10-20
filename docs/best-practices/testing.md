# Testing Strategies

Testing is essential for ensuring the correctness and reliability of applications using Raiqub Expressions. This guide covers strategies for testing specifications, query strategies, and database sessions.

## Overview

Raiqub Expressions is designed to be testable through several key approaches:

- **Unit Testing** - Test specifications and query strategies in isolation using in-memory collections
- **Integration Testing** - Test database interactions with real or test databases
- **Query Translation Testing** - Verify that specifications and query strategies translate correctly to SQL
- **Mocking** - Use mocking frameworks to isolate components
- **Test Fixtures** - Create reusable test data and setup

## Testing Specifications

Specifications are highly testable because they encapsulate business rules in pure functions.

### Unit Testing Specifications

Test specifications using the `IsSatisfiedBy` method with in-memory data:

::: code-group

```csharp [xUnit]
public class ProductSpecificationTests
{
    [Fact]
    public void IsInStock_WithPositiveQuantity_ShouldReturnTrue()
    {
        // Arrange
        var product = new Product { AvailableQuantity = 10 };
        var specification = ProductSpecification.IsInStock;

        // Act
        var result = specification.IsSatisfiedBy(product);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(-1, false)]
    [InlineData(1, true)]
    [InlineData(100, true)]
    public void IsInStock_VariousQuantities_ReturnsExpectedResult(
        int quantity, bool expected)
    {
        // Arrange
        var product = new Product { AvailableQuantity = quantity };
        var specification = ProductSpecification.IsInStock;

        // Act
        var result = specification.IsSatisfiedBy(product);

        // Assert
        Assert.Equal(expected, result);
    }
}
```

```csharp [NUnit]
[TestFixture]
public class ProductSpecificationTests
{
    [Test]
    public void IsInStock_WithPositiveQuantity_ShouldReturnTrue()
    {
        // Arrange
        var product = new Product { AvailableQuantity = 10 };
        var specification = ProductSpecification.IsInStock;

        // Act
        var result = specification.IsSatisfiedBy(product);

        // Assert
        Assert.That(result, Is.True);
    }

    [TestCase(0, false)]
    [TestCase(-1, false)]
    [TestCase(1, true)]
    [TestCase(100, true)]
    public void IsInStock_VariousQuantities_ReturnsExpectedResult(
        int quantity, bool expected)
    {
        // Arrange
        var product = new Product { AvailableQuantity = quantity };
        var specification = ProductSpecification.IsInStock;

        // Act
        var result = specification.IsSatisfiedBy(product);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }
}
```

:::

### Testing Combined Specifications

Test specification composition using logical operators:

```csharp
public class CombinedSpecificationTests
{
    [Fact]
    public void AndSpecification_WhenBothConditionsMet_ShouldReturnTrue()
    {
        // Arrange
        var product = new Product
        {
            AvailableQuantity = 10,
            Price = 25.0m,
            Category = "Electronics"
        };

        var specification = ProductSpecification.IsInStock
            .And(ProductSpecification.InPriceRange(10.0m, 50.0m))
            .And(ProductSpecification.InCategory("Electronics"));

        // Act
        var result = specification.IsSatisfiedBy(product);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void OrSpecification_WhenEitherConditionMet_ShouldReturnTrue()
    {
        // Arrange
        var product = new Product
        {
            AvailableQuantity = 0,
            IsDiscounted = true
        };

        var specification = ProductSpecification.IsInStock
            .Or(ProductSpecification.IsDiscounted);

        // Act
        var result = specification.IsSatisfiedBy(product);

        // Assert
        Assert.True(result); // True because IsDiscounted is true
    }

    [Fact]
    public void NotSpecification_ShouldInvertResult()
    {
        // Arrange
        var product = new Product { AvailableQuantity = 0 };
        var specification = ProductSpecification.IsInStock.Not();

        // Act
        var result = specification.IsSatisfiedBy(product);

        // Assert
        Assert.True(result); // True because product is NOT in stock
    }
}
```

### Testing Query Translation with Entity Framework

When using Entity Framework, you can use the `ToQueryString()` extension method to verify that specifications translate correctly to SQL:

```csharp
public class SpecificationTranslationTests
{
    private readonly TestDbContext _context;

    public SpecificationTranslationTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TestDb")
            .Options;
        _context = new TestDbContext(options);
    }

    [Fact]
    public void Specification_ShouldTranslateToValidSql()
    {
        // Arrange
        var specification = ProductSpecification.IsInStock;

        // Act
        var query = _context.Set<Product>().Where(specification.ToExpression());
        var sql = query.ToQueryString();

        // Assert
        Assert.NotNull(sql);
        Assert.Contains("WHERE", sql);
        Assert.Contains("AvailableQuantity", sql);
        _testOutputHelper.WriteLine(sql); // View generated SQL
    }

    [Fact]
    public void CombinedSpecification_ShouldTranslateToValidSql()
    {
        // Arrange
        var specification = ProductSpecification.IsInStock
            .And(ProductSpecification.InPriceRange(10.0m, 50.0m));

        // Act
        var query = _context.Set<Product>().Where(specification.ToExpression());
        var sql = query.ToQueryString();

        // Assert
        Assert.NotNull(sql);
        Assert.Contains("AND", sql);
        Assert.Contains("AvailableQuantity", sql);
        Assert.Contains("Price", sql);
        _testOutputHelper.WriteLine(sql);
    }

    [Fact]
    public void ComplexSpecification_ShouldNotThrowTranslationException()
    {
        // Arrange
        var specification = ProductSpecification.IsInStock
            .And(ProductSpecification.InCategory("Electronics"))
            .Or(ProductSpecification.IsDiscounted);

        // Act & Assert
        var query = _context.Set<Product>().Where(specification.ToExpression());

        // Should not throw InvalidOperationException
        var exception = Record.Exception(() => query.ToQueryString());
        Assert.Null(exception);
    }
}
```

## Testing Query Strategies

Query strategies define query logic that can be tested independently of the database.

### Unit Testing Query Strategies

Test query strategies with in-memory collections:

```csharp
public class GetProductsQueryStrategyTests
{
    [Fact]
    public void Execute_ShouldFilterInStockProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), Name = "Product A", AvailableQuantity = 10 },
            new() { Id = Guid.NewGuid(), Name = "Product B", AvailableQuantity = 0 },
            new() { Id = Guid.NewGuid(), Name = "Product C", AvailableQuantity = 5 }
        }.AsQueryable();

        var strategy = new GetProductsQueryStrategy();

        // Act
        var result = strategy.Execute(products).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, p => Assert.True(p.AvailableQuantity > 0));
    }

    [Fact]
    public void Execute_ShouldProjectToDto()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), Name = "Product A", Price = 10.0m, AvailableQuantity = 5 }
        }.AsQueryable();

        var strategy = new GetProductDtoQueryStrategy();

        // Act
        var result = strategy.Execute(products).ToList();

        // Assert
        Assert.Single(result);
        Assert.IsType<ProductDto>(result[0]);
        Assert.Equal("Product A", result[0].Name);
    }

    [Fact]
    public void Execute_ShouldApplyOrderingCorrectly()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Name = "Zebra", AvailableQuantity = 10 },
            new() { Name = "Apple", AvailableQuantity = 5 },
            new() { Name = "Mango", AvailableQuantity = 3 }
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

### Testing Query Strategy Translation with Entity Framework

Verify that query strategies translate correctly to SQL:

```csharp
public class QueryStrategyTranslationTests
{
    private readonly TestDbContext _context;
    private readonly ITestOutputHelper _testOutputHelper;

    public QueryStrategyTranslationTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TestDb")
            .Options;
        _context = new TestDbContext(options);
    }

    [Fact]
    public void QueryStrategy_ShouldTranslateToValidSql()
    {
        // Arrange
        var strategy = new GetProductsQueryStrategy();
        var source = _context.Set<Product>().AsQueryable();

        // Act
        var query = strategy.Execute(source);
        var sql = query.ToQueryString();

        // Assert
        Assert.NotNull(sql);
        Assert.Contains("SELECT", sql);
        _testOutputHelper.WriteLine(sql);
    }

    [Fact]
    public void QueryStrategy_WithJoins_ShouldTranslateCorrectly()
    {
        // Arrange
        var strategy = new GetOrdersWithCustomerQueryStrategy();
        var querySource = new EfQuerySource(_context);

        // Act
        var query = strategy.Execute(querySource);
        var sql = query.ToQueryString();

        // Assert
        Assert.Contains("JOIN", sql);
        Assert.Contains("Customer", sql);
        _testOutputHelper.WriteLine(sql);
    }

    [Fact]
    public void QueryStrategy_WithComplexProjection_ShouldNotThrow()
    {
        // Arrange
        var strategy = new GetProductWithCategoryQueryStrategy();
        var source = _context.Set<Product>().AsQueryable();

        // Act & Assert
        var query = strategy.Execute(source);
        var exception = Record.Exception(() => query.ToQueryString());

        Assert.Null(exception);
    }

    [Fact]
    public void QueryStrategy_CheckGeneratedSqlForPerformance()
    {
        // Arrange
        var strategy = new GetProductsWithMultipleJoinsQueryStrategy();
        var source = _context.Set<Product>().AsQueryable();

        // Act
        var query = strategy.Execute(source);
        var sql = query.ToQueryString();

        // Assert - Verify no N+1 queries
        Assert.DoesNotContain("SELECT", sql.Substring(sql.IndexOf("FROM")));

        // Assert - Verify proper JOINs instead of subqueries
        var joinCount = Regex.Matches(sql, "JOIN").Count;
        Assert.InRange(joinCount, 1, 3);

        _testOutputHelper.WriteLine(sql);
    }
}
```

### Testing Parameterized Query Strategies

Test query strategies with different parameters:

```csharp
public class SearchProductsQueryStrategyTests
{
    [Theory]
    [InlineData("Apple", 1)]
    [InlineData("Banana", 1)]
    [InlineData("Orange", 0)]
    public void Execute_WithNameFilter_ShouldReturnMatchingProducts(
        string nameFilter, int expectedCount)
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Name = "Apple" },
            new() { Name = "Banana" },
            new() { Name = "Apple Juice" }
        }.AsQueryable();

        var strategy = new SearchProductsQueryStrategy(nameFilter: nameFilter);

        // Act
        var result = strategy.Execute(products).ToList();

        // Assert
        Assert.Equal(expectedCount, result.Count);
    }

    [Fact]
    public void Execute_WithPriceRange_ShouldFilterCorrectly()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Price = 5.0m },
            new() { Price = 15.0m },
            new() { Price = 25.0m },
            new() { Price = 35.0m }
        }.AsQueryable();

        var strategy = new SearchProductsQueryStrategy(minPrice: 10.0m, maxPrice: 30.0m);

        // Act
        var result = strategy.Execute(products).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, p => Assert.InRange(p.Price, 10.0m, 30.0m));
    }
}
```

## Testing Database Sessions

Test database sessions using integration tests with real or in-memory databases.

### Integration Testing with EF Core In-Memory

Use EF Core's in-memory provider for fast integration tests:

```csharp
public class ProductServiceTests : IDisposable
{
    private readonly IDbSession _session;
    private readonly IServiceProvider _serviceProvider;

    public ProductServiceTests()
    {
        var services = new ServiceCollection();

        // Configure in-memory database
        services.AddDbContextFactory<TestDbContext>(options =>
            options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}"));

        services.AddEntityFrameworkExpressions()
            .AddSingleContext<TestDbContext>();

        _serviceProvider = services.BuildServiceProvider();
        _session = _serviceProvider.GetRequiredService<IDbSession>();
    }

    [Fact]
    public async Task CreateProduct_ShouldPersistToDatabase()
    {
        // Arrange
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            Price = 10.0m,
            AvailableQuantity = 5
        };

        // Act
        _session.Add(product);
        await _session.SaveChangesAsync();

        // Assert
        var query = _session.Query<Product>().Where(p => p.Id == product.Id);
        var retrieved = await query.FirstAsync();

        Assert.NotNull(retrieved);
        Assert.Equal(product.Name, retrieved.Name);
    }

    [Fact]
    public async Task Query_WithSpecification_ShouldFilterCorrectly()
    {
        // Arrange
        _session.Add(new Product { Name = "Product 1", AvailableQuantity = 10 });
        _session.Add(new Product { Name = "Product 2", AvailableQuantity = 0 });
        await _session.SaveChangesAsync();

        // Act
        var query = _session.Query(ProductSpecification.IsInStock);
        var results = await query.ToListAsync();

        // Assert
        Assert.Single(results);
        Assert.Equal("Product 1", results[0].Name);
    }

    public void Dispose()
    {
        (_serviceProvider as IDisposable)?.Dispose();
    }
}
```

### Integration Testing with SQLite

Use SQLite for more realistic integration tests:

```csharp
public class ProductServiceIntegrationTests : IAsyncLifetime
{
    private readonly IDbSession _session;
    private readonly IServiceProvider _serviceProvider;
    private readonly SqliteConnection _connection;

    public ProductServiceIntegrationTests()
    {
        var services = new ServiceCollection();

        // Create in-memory SQLite connection
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        services.AddDbContextFactory<TestDbContext>(options =>
            options.UseSqlite(_connection));

        services.AddEntityFrameworkExpressions()
            .AddSingleContext<TestDbContext>();

        _serviceProvider = services.BuildServiceProvider();
        _session = _serviceProvider.GetRequiredService<IDbSession>();
    }

    public async Task InitializeAsync()
    {
        // Create database schema
        var context = _serviceProvider.GetRequiredService<IDbContextFactory<TestDbContext>>()
            .CreateDbContext();
        await context.Database.EnsureCreatedAsync();
    }

    [Fact]
    public async Task Query_WithComplexSpecification_ShouldWork()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test Product",
            Price = 25.0m,
            AvailableQuantity = 10,
            Category = "Electronics"
        };

        _session.Add(product);
        await _session.SaveChangesAsync();

        // Act
        var specification = ProductSpecification.IsInStock
            .And(ProductSpecification.InPriceRange(20.0m, 30.0m))
            .And(ProductSpecification.InCategory("Electronics"));

        var query = _session.Query(specification);
        var results = await query.ToListAsync();

        // Assert
        Assert.Single(results);
    }

    public async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
        if (_serviceProvider is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }
    }
}
```

### Integration Testing with Testcontainers

Use Testcontainers for testing with real databases:

```csharp
public class ProductServiceDockerTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres;
    private IServiceProvider _serviceProvider;
    private IDbSession _session;

    public ProductServiceDockerTests()
    {
        _postgres = new PostgreSqlBuilder()
            .WithImage("postgres:15-alpine")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        var services = new ServiceCollection();

        services.AddDbContextFactory<TestDbContext>(options =>
            options.UseNpgsql(_postgres.GetConnectionString()));

        services.AddEntityFrameworkExpressions()
            .AddSingleContext<TestDbContext>();

        _serviceProvider = services.BuildServiceProvider();
        _session = _serviceProvider.GetRequiredService<IDbSession>();

        // Create schema
        var context = _serviceProvider.GetRequiredService<IDbContextFactory<TestDbContext>>()
            .CreateDbContext();
        await context.Database.EnsureCreatedAsync();
    }

    [Fact]
    public async Task Query_AgainstRealPostgres_ShouldWork()
    {
        // Arrange
        _session.Add(new Product { Name = "Test Product", AvailableQuantity = 10 });
        await _session.SaveChangesAsync();

        // Act
        var query = _session.Query(ProductSpecification.IsInStock);
        var results = await query.ToListAsync();

        // Assert
        Assert.Single(results);
    }

    public async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
    }
}
```

## Mocking Database Sessions

Use mocking frameworks to isolate components from database dependencies.

### Mocking with Moq

```csharp
public class ProductServiceTests
{
    [Fact]
    public async Task GetProducts_ShouldReturnProducts()
    {
        // Arrange
        var mockSession = new Mock<IDbQuerySession>();
        var mockQuery = new Mock<IDbQuery<Product>>();

        var expectedProducts = new List<Product>
        {
            new() { Id = Guid.NewGuid(), Name = "Product 1" },
            new() { Id = Guid.NewGuid(), Name = "Product 2" }
        };

        mockQuery
            .Setup(q => q.ToListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedProducts);

        mockSession
            .Setup(s => s.Query<Product>())
            .Returns(mockQuery.Object);

        var service = new ProductService(mockSession.Object);

        // Act
        var result = await service.GetProductsAsync();

        // Assert
        Assert.Equal(2, result.Count);
        mockSession.Verify(s => s.Query<Product>(), Times.Once);
    }

    [Fact]
    public async Task CreateProduct_ShouldCallAddAndSaveChanges()
    {
        // Arrange
        var mockSession = new Mock<IDbSession>();
        var product = new Product { Name = "New Product" };

        mockSession
            .Setup(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var service = new ProductService(mockSession.Object);

        // Act
        await service.CreateProductAsync(product);

        // Assert
        mockSession.Verify(s => s.Add(product), Times.Once);
        mockSession.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
```

### Mocking with NSubstitute

```csharp
public class OrderServiceTests
{
    [Fact]
    public async Task GetOrder_ShouldReturnOrder()
    {
        // Arrange
        var session = Substitute.For<IDbQuerySession>();
        var query = Substitute.For<IDbQuery<Order>>();

        var expectedOrder = new Order { Id = Guid.NewGuid() };

        query.FirstAsync(Arg.Any<CancellationToken>())
            .Returns(expectedOrder);

        session.Query<Order>()
            .Returns(query);

        var service = new OrderService(session);

        // Act
        var result = await service.GetOrderAsync(expectedOrder.Id);

        // Assert
        Assert.Equal(expectedOrder.Id, result.Id);
        await query.Received(1).FirstAsync(Arg.Any<CancellationToken>());
    }
}
```

## Test Data Builders

Create test data builders for reusable test data:

```csharp
public class ProductBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _name = "Test Product";
    private decimal _price = 10.0m;
    private int _availableQuantity = 10;
    private string _category = "Test Category";

    public ProductBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public ProductBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ProductBuilder WithPrice(decimal price)
    {
        _price = price;
        return this;
    }

    public ProductBuilder OutOfStock()
    {
        _availableQuantity = 0;
        return this;
    }

    public ProductBuilder InStock(int quantity = 10)
    {
        _availableQuantity = quantity;
        return this;
    }

    public ProductBuilder InCategory(string category)
    {
        _category = category;
        return this;
    }

    public Product Build()
    {
        return new Product
        {
            Id = _id,
            Name = _name,
            Price = _price,
            AvailableQuantity = _availableQuantity,
            Category = _category
        };
    }
}

// Usage
public class ProductTests
{
    [Fact]
    public void Specification_WithBuilder_ShouldWork()
    {
        // Arrange
        var product = new ProductBuilder()
            .WithName("Electronics Item")
            .InStock(5)
            .InCategory("Electronics")
            .Build();

        // Act
        var result = ProductSpecification.IsInStock.IsSatisfiedBy(product);

        // Assert
        Assert.True(result);
    }
}
```

## Testing Best Practices

### Do's

✅ **Test business logic** - Focus on testing specifications and query strategies  
✅ **Use in-memory tests** - For fast unit tests of query logic  
✅ **Use real databases** - For critical integration tests  
✅ **Test SQL translation** - Use `ToQueryString()` to verify EF Core query generation  
✅ **Test edge cases** - Null values, empty collections, boundary conditions  
✅ **Use test builders** - For creating consistent test data  
✅ **Mock external dependencies** - Isolate the code under test  
✅ **Test error handling** - Verify exception handling and error scenarios  
✅ **Use descriptive test names** - Make test intent clear  
✅ **Review generated SQL** - Check for performance issues like N+1 queries  

### Don'ts

❌ **Don't test framework code** - Trust that EF Core/Marten work correctly  
❌ **Don't overuse mocks** - Prefer integration tests for database operations  
❌ **Don't share state** - Each test should be independent  
❌ **Don't test implementation details** - Test behavior, not internal structure  
❌ **Don't skip cleanup** - Always dispose resources properly  
❌ **Don't ignore async/await** - Use proper async testing patterns  
❌ **Don't assume SQL correctness** - Always verify with `ToQueryString()` in EF tests  

## Testing Patterns

### Arrange-Act-Assert (AAA)

Structure tests using the AAA pattern:

```csharp
[Fact]
public async Task GetProducts_ReturnsInStockProducts()
{
    // Arrange - Set up test data and dependencies
    var product = new ProductBuilder().InStock().Build();
    _session.Add(product);
    await _session.SaveChangesAsync();

    // Act - Execute the code under test
    var query = _session.Query(ProductSpecification.IsInStock);
    var results = await query.ToListAsync();

    // Assert - Verify the results
    Assert.Single(results);
    Assert.Equal(product.Id, results[0].Id);
}
```

### Test Fixtures

Share setup across tests using fixtures:

```csharp
public class DatabaseFixture : IAsyncLifetime
{
    private IServiceProvider _serviceProvider;
    private AsyncServiceScope _serviceScope;

    public IDbSession Session { get; private set; }

    public async Task InitializeAsync()
    {
        var services = new ServiceCollection();
        services.AddDbContextFactory<TestDbContext>(options =>
            options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}"));
        services.AddEntityFrameworkExpressions().AddSingleContext<TestDbContext>();

        _serviceProvider = services.BuildServiceProvider();
        _serviceScope = _serviceProvider.CreateAsyncScope();
        Session = _serviceScope.ServiceProvider.GetRequiredService<IDbSession>();
    }

    public async Task DisposeAsync()
    {
        await _serviceScope.DisposeAsync();
        await _serviceProvider.DisposeAsync();
    }
}

public class ProductTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public ProductTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Test_UsesSharedFixture()
    {
        var query = _fixture.Session.Query<Product>();
        var results = await query.ToListAsync();
        Assert.NotNull(results);
    }
}
```

## See Also

- [Specification](/specification/) - Testing specifications
- [Query Strategy](/query-strategy/) - Testing query strategies
- [Database Session](/database-session/) - Session usage
- [Entity Framework Core](/ef-core/) - EF Core integration
- [xUnit](https://xunit.net/) - Testing framework
- [Moq](https://github.com/moq/moq4) - Mocking framework
- [Testcontainers](https://dotnet.testcontainers.org/) - Integration testing with Docker
- [EF Core Testing](https://learn.microsoft.com/en-us/ef/core/testing/) - Official EF Core testing guide
