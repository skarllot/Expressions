# Bounded Contexts

Bounded contexts are a strategic design pattern from Domain-Driven Design (DDD) that helps manage complexity in large applications by dividing the domain model into distinct contexts, each with its own ubiquitous language and clear boundaries.

Raiqub Expressions provides typed session interfaces to work with multiple bounded contexts, ensuring type safety and isolation between different contexts.

## What is a Bounded Context?

A bounded context is a clear boundary within which a domain model is defined and used. It is responsible for defining a ubiquitous language, which is a shared set of terms and concepts used by all members of the project team and domain experts to communicate about the domain.

Key characteristics:
- **Clear boundaries** - Each context has explicit boundaries defining what's inside and outside
- **Ubiquitous language** - Consistent terminology used within the context
- **Isolation** - Models in different contexts can have different representations of the same concept
- **Integration points** - Well-defined interfaces for context communication

## Why Use Bounded Contexts?

### Benefits

1. **Type Safety** - Compile-time verification that you're using the correct context
2. **Separation of Concerns** - Each context handles its own domain logic
3. **Team Autonomy** - Different teams can work on different contexts independently
4. **Model Clarity** - Prevents model ambiguity by isolating different interpretations
5. **Scalability** - Easier to scale different parts of the system independently

### When to Use

Consider using bounded contexts when:
- Your application has multiple business domains or subdomains
- Different parts of the system have different models for the same concepts
- You want to prevent accidental coupling between different areas
- Multiple teams are working on the same codebase
- You're implementing a microservices architecture

## Defining Bounded Contexts

Define marker interfaces to represent your bounded contexts:

```csharp
// Catalog context - manages products, categories, inventory
public interface ICatalogContext
{
}

// Sales context - manages orders, customers, pricing
public interface ISalesContext
{
}

// Reporting context - read-only analytics and reports
public interface IReportingContext
{
}
```

These interfaces serve as type parameters to ensure sessions are used with the correct context.

## Typed Session Interfaces

Raiqub Expressions provides generic versions of session interfaces for bounded contexts:

### IDbQuerySession&lt;TContext&gt;

For read-only operations within a bounded context:

```csharp
public interface IDbQuerySession<out TContext> : IDbQuerySession
{
    /// <summary>Gets the bounded context associated with this query session.</summary>
    TContext Context { get; }
}
```

### IDbSession&lt;TContext&gt;

For read and write operations within a bounded context:

```csharp
public interface IDbSession<out TContext> : IDbQuerySession<TContext>, IDbSession
{
}
```

### IDbQuerySessionFactory&lt;TContext&gt;

Factory for creating query sessions for a specific bounded context:

```csharp
public interface IDbQuerySessionFactory<out TContext>
{
    IDbQuerySession<TContext> Create();
}
```

### IDbSessionFactory&lt;TContext&gt;

Factory for creating read/write sessions for a specific bounded context:

```csharp
public interface IDbSessionFactory<out TContext>
{
    IDbSession<TContext> Create(ChangeTracking? tracking = null);
}
```

## Using Bounded Context Sessions

### Dependency Injection

Inject typed sessions into your services:

::: code-group

```csharp [Read-Only Session]
public class CatalogQueryService
{
    private readonly IDbQuerySession<ICatalogContext> _session;

    public CatalogQueryService(IDbQuerySession<ICatalogContext> session)
    {
        _session = session;
    }

    public async Task<IReadOnlyList<Product>> GetProductsAsync(
        CancellationToken cancellationToken = default)
    {
        var query = _session.Query<Product>();
        return await query.ToListAsync(cancellationToken);
    }
}
```

```csharp [Read/Write Session]
public class SalesCommandService
{
    private readonly IDbSession<ISalesContext> _session;

    public SalesCommandService(IDbSession<ISalesContext> session)
    {
        _session = session;
    }

    public async Task CreateOrderAsync(Order order, CancellationToken cancellationToken = default)
    {
        _session.Add(order);
        await _session.SaveChangesAsync(cancellationToken);
    }
}
```

```csharp [Session Factory]
public class OrderReportService
{
    private readonly IDbQuerySessionFactory<ISalesContext> _sessionFactory;

    public OrderReportService(IDbQuerySessionFactory<ISalesContext> sessionFactory)
    {
        _sessionFactory = sessionFactory;
    }

    public async Task<OrderReport> GenerateReportAsync(
        DateTimeOffset start,
        DateTimeOffset end,
        CancellationToken cancellationToken = default)
    {
        await using var session = _sessionFactory.Create();

        var query = session.Query(new GetOrdersInDateRangeQueryStrategy(start, end));
        var orders = await query.ToListAsync(cancellationToken);

        return new OrderReport(orders);
    }
}
```

:::

### Accessing Context Information

Access the underlying context (DbContext or IDocumentStore) if needed:

```csharp
public class CatalogService
{
    private readonly IDbQuerySession<ICatalogContext> _session;

    public CatalogService(IDbQuerySession<ICatalogContext> session)
    {
        _session = session;
    }

    public void LogContext()
    {
        ICatalogContext context = _session.Context;
        Console.WriteLine($"Context: {context.Name}");
    }
}
```

## Cross-Context Operations

Sometimes you need to coordinate operations across multiple contexts.

### Application Services Coordination

Create application services that coordinate multiple contexts:

```csharp
public class OrderFulfillmentService
{
    private readonly IDbSession<ISalesContext> _salesSession;
    private readonly IDbSession<ICatalogContext> _catalogSession;
    private readonly ILogger<OrderFulfillmentService> _logger;

    public OrderFulfillmentService(
        IDbSession<ISalesContext> salesSession,
        IDbSession<ICatalogContext> catalogSession,
        ILogger<OrderFulfillmentService> logger)
    {
        _salesSession = salesSession;
        _catalogSession = catalogSession;
        _logger = logger;
    }

    public async Task<Result> ProcessOrderAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Get order from sales context
            var orderQuery = _salesSession.Query<Order>()
                .Where(o => o.Id == orderId);
            var order = await orderQuery.FirstAsync(cancellationToken);

            // Update inventory in catalog context
            foreach (var item in order.Items)
            {
                var productQuery = _catalogSession.Query<Product>()
                    .Where(p => p.Id == item.ProductId);
                var product = await productQuery.FirstAsync(cancellationToken);

                product.AvailableQuantity -= item.Quantity;
                _catalogSession.Update(product);
            }

            // Update order status in sales context
            order.Status = OrderStatus.Fulfilled;
            _salesSession.Update(order);

            // Save both contexts
            await _catalogSession.SaveChangesAsync(cancellationToken);
            await _salesSession.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process order {OrderId}", orderId);
            return Result.Failure("Order processing failed");
        }
    }
}
```

### Event-Driven Integration

Use domain events to integrate contexts asynchronously:

```csharp
public class OrderPlacedEventHandler
{
    private readonly IDbSession<ICatalogContext> _catalogSession;
    private readonly ILogger<OrderPlacedEventHandler> _logger;

    public OrderPlacedEventHandler(
        IDbSession<ICatalogContext> catalogSession,
        ILogger<OrderPlacedEventHandler> logger)
    {
        _catalogSession = catalogSession;
        _logger = logger;
    }

    public async Task Handle(OrderPlacedEvent evt, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating inventory for order {OrderId}", evt.OrderId);

        foreach (var item in evt.Items)
        {
            var productQuery = _catalogSession.Query<Product>()
                .Where(p => p.Id == item.ProductId);
            var product = await productQuery.FirstAsync(cancellationToken);

            product.ReservedQuantity += item.Quantity;
            _catalogSession.Update(product);
        }

        await _catalogSession.SaveChangesAsync(cancellationToken);
    }
}
```

### Anti-Corruption Layer

Create an anti-corruption layer to translate between contexts:

```csharp
public class CatalogToSalesAdapter
{
    private readonly IDbQuerySession<ICatalogContext> _catalogSession;

    public CatalogToSalesAdapter(IDbQuerySession<ICatalogContext> catalogSession)
    {
        _catalogSession = catalogSession;
    }

    public async Task<SalesProduct> GetSalesProductAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        // Query from catalog context
        var query = _catalogSession.Query<Product>()
            .Where(p => p.Id == productId);
        var catalogProduct = await query.FirstAsync(cancellationToken);

        // Translate to sales context model
        return new SalesProduct
        {
            ProductId = catalogProduct.Id,
            Name = catalogProduct.Name,
            Price = catalogProduct.Price,
            IsAvailable = catalogProduct.AvailableQuantity > 0
        };
    }
}
```

## Testing with Bounded Contexts

### Unit Testing

Mock the typed sessions in unit tests:

```csharp
public class CatalogQueryServiceTests
{
    [Fact]
    public async Task GetProductsAsync_ShouldReturnProducts()
    {
        // Arrange
        var mockSession = new Mock<IDbQuerySession<ICatalogContext>>();
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

        var service = new CatalogQueryService(mockSession.Object);

        // Act
        var result = await service.GetProductsAsync();

        // Assert
        Assert.Equal(2, result.Count);
    }
}
```

### Integration Testing

Test with in-memory or test databases:

```csharp
public class CatalogIntegrationTests : IAsyncLifetime
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDbSession<ICatalogContext> _session;

    public CatalogIntegrationTests()
    {
        var services = new ServiceCollection();

        // Setup test database
        services.AddDbContextFactory<CatalogDbContext>(options =>
            options.UseInMemoryDatabase("CatalogTest"));

        services.AddEntityFrameworkExpressions()
            .AddContext<ICatalogContext, CatalogDbContext>();

        _serviceProvider = services.BuildServiceProvider();
        _session = _serviceProvider.GetRequiredService<IDbSession<ICatalogContext>>();
    }

    public async Task InitializeAsync()
    {
        // Seed test data
        _session.Add(new Product { Id = Guid.NewGuid(), Name = "Test Product" });
        await _session.SaveChangesAsync();
    }

    [Fact]
    public async Task Should_Query_Products_From_Catalog_Context()
    {
        // Arrange
        var query = _session.Query<Product>();

        // Act
        var products = await query.ToListAsync();

        // Assert
        Assert.NotEmpty(products);
    }

    public async Task DisposeAsync()
    {
        if (_serviceProvider is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }
    }
}
```

## Best Practices

### Do's

✅ **Use meaningful context names** - Names should reflect the business domain  
✅ **Keep contexts independent** - Minimize dependencies between contexts  
✅ **Define clear boundaries** - Be explicit about what belongs in each context  
✅ **Use domain events** - For asynchronous cross-context communication  
✅ **Create adapters** - For translating between different context models  
✅ **Document context interactions** - Make integration points explicit  

### Don'ts

❌ **Don't share entities** - Each context should have its own entity definitions  
❌ **Don't directly reference** - Avoid direct dependencies between context implementations  
❌ **Don't use distributed transactions** - Prefer eventual consistency  
❌ **Don't create too many contexts** - Start simple and split when needed  
❌ **Don't mix concerns** - Keep business logic within appropriate contexts  

## Common Patterns

### CQRS with Bounded Contexts

Separate read and write models within contexts:

```csharp
// Write model in sales context
public class CreateOrderCommandHandler
{
    private readonly IDbSession<ISalesContext> _session;

    public CreateOrderCommandHandler(IDbSession<ISalesContext> session)
    {
        _session = session;
    }

    public async Task<Guid> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = command.CustomerId,
            Items = command.Items
        };

        _session.Add(order);
        await _session.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}

// Read model in reporting context
public class GetOrderDetailsQueryHandler
{
    private readonly IDbQuerySession<IReportingContext> _session;

    public GetOrderDetailsQueryHandler(IDbQuerySession<IReportingContext> session)
    {
        _session = session;
    }

    public async Task<OrderDetails> Handle(GetOrderDetailsQuery query, CancellationToken cancellationToken)
    {
        var dbQuery = _session.Query(new GetOrderDetailsQueryStrategy(query.OrderId));
        return await dbQuery.FirstAsync(cancellationToken);
    }
}
```

## Registration and Configuration

For detailed instructions on registering and configuring bounded contexts with specific ORMs:

- [Entity Framework Core Bounded Contexts](/ef-core/bounded-contexts)
- [Marten Bounded Contexts](/marten/bounded-contexts)
