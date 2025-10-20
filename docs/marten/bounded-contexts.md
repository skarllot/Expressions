# Marten Bounded Contexts

This guide explains how to configure and use bounded contexts with Marten in Raiqub Expressions.

## Prerequisites

Before working with bounded contexts, ensure you have:

1. **Marten** configured in your project
2. **Raiqub.Expressions.Marten** package installed
3. PostgreSQL database configured
4. Document stores defined for your contexts

## Registering Bounded Contexts

### Single Document Store per Context

The recommended approach is to have separate Marten document stores for each bounded context:

```csharp
// Define bounded context interfaces
public interface ICatalogContext { }
public interface ISalesContext { }
public interface IReportingContext { }

// Define document store wrappers (optional, for explicit typing)
public class CatalogDocumentStore : IDocumentStore, ICatalogContext
{
    private readonly IDocumentStore _inner;

    public CatalogDocumentStore(IDocumentStore inner)
    {
        _inner = inner;
    }

    // Delegate all IDocumentStore methods to _inner
    public IDocumentSession OpenSession(SessionOptions options = null) =>
        _inner.OpenSession(options);

    // ... other IDocumentStore members
}
```

### Register with Dependency Injection

Using Marten's built-in multi-tenancy or multiple store support:

```csharp
// Register multiple Marten stores
services.AddMarten("catalog", options =>
{
    options.Connection("Host=localhost;Database=catalog;Username=postgres;Password=password");
    options.DatabaseSchemaName = "catalog";

    // Configure catalog documents
    options.Schema.For<Product>().Identity(x => x.Id);
    options.Schema.For<Category>().Identity(x => x.Id);
});

services.AddMarten("sales", options =>
{
    options.Connection("Host=localhost;Database=sales;Username=postgres;Password=password");
    options.DatabaseSchemaName = "sales";

    // Configure sales documents
    options.Schema.For<Order>().Identity(x => x.Id);
    options.Schema.For<Customer>().Identity(x => x.Id);
});

// Register bounded context sessions
services.AddMartenExpressions()
    .AddContext<ICatalogContext, IDocumentStore>() // Resolves "catalog" store
    .AddContext<ISalesContext, IDocumentStore>();   // Resolves "sales" store
```

### Single Store with Schema Separation

Alternatively, use a single document store with schema separation:

```csharp
services.AddMarten(options =>
{
    options.Connection("Host=localhost;Database=myapp;Username=postgres;Password=password");

    // Catalog documents in 'catalog' schema
    options.Schema.For<Product>()
        .Identity(x => x.Id)
        .DatabaseSchemaName("catalog");

    options.Schema.For<Category>()
        .Identity(x => x.Id)
        .DatabaseSchemaName("catalog");

    // Sales documents in 'sales' schema
    options.Schema.For<Order>()
        .Identity(x => x.Id)
        .DatabaseSchemaName("sales");

    options.Schema.For<Customer>()
        .Identity(x => x.Id)
        .DatabaseSchemaName("sales");
});

services.AddMartenExpressions()
    .AddContext<ICatalogContext, IDocumentStore>()
    .AddContext<ISalesContext, IDocumentStore>();
```

## Configuration Options

### Change Tracking

Configure change tracking behavior per context:

```csharp
services.AddMartenExpressions()
    // Write context with tracking enabled
    .AddContext<ICatalogContext, IDocumentStore>(ChangeTracking.Enabled)
    // Read-only context with tracking disabled
    .AddContext<IReportingContext, IDocumentStore>(ChangeTracking.Disabled);
```

### Document Configuration

Configure document-specific options:

```csharp
services.AddMarten(options =>
{
    options.Connection(connectionString);
    options.DatabaseSchemaName = "catalog";

    // Configure Product document
    options.Schema.For<Product>()
        .Identity(x => x.Id)
        .UseOptimisticConcurrency(true)
        .Index(x => x.Category)
        .Index(x => x.Price)
        .SoftDeleted();

    // Configure Category document
    options.Schema.For<Category>()
        .Identity(x => x.Id)
        .UniqueIndex(x => x.Name);
});
```

## Using Bounded Context Sessions

### Injecting Sessions

Inject the typed session interfaces into your services:

```csharp
public class CatalogService
{
    private readonly IDbSession<ICatalogContext> _session;
    private readonly ILogger<CatalogService> _logger;

    public CatalogService(
        IDbSession<ICatalogContext> session,
        ILogger<CatalogService> logger)
    {
        _session = session;
        _logger = logger;
    }

    public async Task<Product> CreateProductAsync(
        Product product,
        CancellationToken cancellationToken = default)
    {
        _session.Add(product);
        await _session.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<IReadOnlyList<Product>> GetProductsAsync(
        CancellationToken cancellationToken = default)
    {
        var query = _session.Query<Product>();
        return await query.ToListAsync(cancellationToken);
    }
}
```

### Using Session Factories

For scenarios requiring explicit session creation:

```csharp
public class OrderService
{
    private readonly IDbSessionFactory<ISalesContext> _sessionFactory;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IDbSessionFactory<ISalesContext> sessionFactory,
        ILogger<OrderService> logger)
    {
        _sessionFactory = sessionFactory;
        _logger = logger;
    }

    public async Task ProcessOrderAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        await using var session = _sessionFactory.Create();

        var query = session.Query<Order>().Where(o => o.Id == orderId);
        var order = await query.FirstAsync(cancellationToken);

        order.Status = OrderStatus.Processed;
        session.Update(order);

        await session.SaveChangesAsync(cancellationToken);
    }
}
```

## Database and Schema Separation

### Separate Databases

Use different connection strings for complete database isolation:

```csharp
services.AddMarten("catalog", options =>
{
    options.Connection("Host=localhost;Database=catalog_db;...");
    options.Schema.For<Product>().Identity(x => x.Id);
});

services.AddMarten("sales", options =>
{
    options.Connection("Host=localhost;Database=sales_db;...");
    options.Schema.For<Order>().Identity(x => x.Id);
});

services.AddMartenExpressions()
    .AddContext<ICatalogContext, IDocumentStore>()
    .AddContext<ISalesContext, IDocumentStore>();
```

### Same Database, Different Schemas

Use PostgreSQL schemas to separate contexts within a single database:

```csharp
var connectionString = "Host=localhost;Database=myapp;Username=postgres;Password=password";

services.AddMarten(options =>
{
    options.Connection(connectionString);

    // Catalog schema
    options.Schema.For<Product>()
        .Identity(x => x.Id)
        .DatabaseSchemaName("catalog");

    options.Schema.For<Category>()
        .Identity(x => x.Id)
        .DatabaseSchemaName("catalog");

    // Sales schema
    options.Schema.For<Order>()
        .Identity(x => x.Id)
        .DatabaseSchemaName("sales");

    options.Schema.For<Customer>()
        .Identity(x => x.Id)
        .DatabaseSchemaName("sales");
});

services.AddMartenExpressions()
    .AddContext<ICatalogContext, IDocumentStore>()
    .AddContext<ISalesContext, IDocumentStore>();
```

## Cross-Context Queries

### Querying Across Contexts

When you need data from multiple contexts, coordinate at the application level:

```csharp
public class OrderFulfillmentService
{
    private readonly IDbSession<ISalesContext> _salesSession;
    private readonly IDbQuerySession<ICatalogContext> _catalogSession;

    public OrderFulfillmentService(
        IDbSession<ISalesContext> salesSession,
        IDbQuerySession<ICatalogContext> catalogSession)
    {
        _salesSession = salesSession;
        _catalogSession = catalogSession;
    }

    public async Task<OrderFulfillmentResult> FulfillOrderAsync(
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        // Get order from sales context
        var orderQuery = _salesSession.Query<Order>()
            .Where(o => o.Id == orderId);
        var order = await orderQuery.FirstAsync(cancellationToken);

        var result = new OrderFulfillmentResult { OrderId = orderId };

        // Check product availability in catalog context
        foreach (var item in order.Items)
        {
            var productQuery = _catalogSession.Query<Product>()
                .Where(p => p.Id == item.ProductId);
            var product = await productQuery.FirstOrDefaultAsync(cancellationToken);

            if (product == null || product.AvailableQuantity < item.Quantity)
            {
                result.Errors.Add($"Product {item.ProductId} not available");
            }
        }

        if (result.Errors.Any())
        {
            return result;
        }

        // Update order status
        order.Status = OrderStatus.Fulfilled;
        _salesSession.Update(order);
        await _salesSession.SaveChangesAsync(cancellationToken);

        result.Success = true;
        return result;
    }
}
```

### Multi-Entity Queries Across Contexts

Use `IQueryStrategy<TResult>` for queries spanning multiple contexts:

```csharp
public class GetOrderWithProductDetailsQueryStrategy : IQueryStrategy<OrderWithProducts>
{
    private readonly Guid _orderId;

    public GetOrderWithProductDetailsQueryStrategy(Guid orderId)
    {
        _orderId = orderId;
    }

    public IQueryable<OrderWithProducts> Execute(IQuerySource source)
    {
        // Get orders from sales context
        var orders = source.GetSet<Order>();

        // Get products from catalog context
        var products = source.GetSet<Product>();

        return from order in orders
               where order.Id == _orderId
               from item in order.Items
               join product in products on item.ProductId equals product.Id
               group product by order into g
               select new OrderWithProducts
               {
                   OrderId = g.Key.Id,
                   OrderDate = g.Key.Date,
                   Products = g.ToList()
               };
    }
}
```

## Marten-Specific Features with Bounded Contexts

### Event Sourcing per Context

Configure event sourcing separately for each bounded context:

```csharp
services.AddMarten("sales", options =>
{
    options.Connection(salesConnectionString);
    options.DatabaseSchemaName = "sales";

    // Enable event sourcing for sales context
    options.Events.DatabaseSchemaName = "sales_events";

    options.Events.AddEventType<OrderPlacedEvent>();
    options.Events.AddEventType<OrderFulfilledEvent>();
});

services.AddMartenExpressions()
    .AddContext<ISalesContext, IDocumentStore>();
```

### Full-Text Search

Use Marten's full-text search within specific contexts:

```csharp
public class SearchCatalogProductsQueryStrategy : EntityQueryStrategy<Product, Product>
{
    private readonly string _searchTerm;

    public SearchCatalogProductsQueryStrategy(string searchTerm)
    {
        _searchTerm = searchTerm;
    }

    protected override IQueryable<Product> ExecuteCore(IQueryable<Product> source)
    {
        // Marten's PlainTextSearch extension
        return source.Where(x => x.Search(_searchTerm));
    }
}

// Usage with catalog context
public class CatalogSearchService
{
    private readonly IDbQuerySession<ICatalogContext> _session;

    public CatalogSearchService(IDbQuerySession<ICatalogContext> session)
    {
        _session = session;
    }

    public async Task<IReadOnlyList<Product>> SearchProductsAsync(
        string searchTerm,
        CancellationToken cancellationToken = default)
    {
        var query = _session.Query(new SearchCatalogProductsQueryStrategy(searchTerm));
        return await query.ToListAsync(cancellationToken);
    }
}
```

### Soft Deletes

Configure soft deletes per context:

```csharp
services.AddMarten(options =>
{
    options.Connection(connectionString);

    // Catalog context with soft deletes
    options.Schema.For<Product>()
        .Identity(x => x.Id)
        .DatabaseSchemaName("catalog")
        .SoftDeleted();

    // Sales context without soft deletes (hard delete)
    options.Schema.For<Order>()
        .Identity(x => x.Id)
        .DatabaseSchemaName("sales");
});
```

## Schema Management

### Apply Schema Changes

Apply schema changes separately for each bounded context:

```csharp
// In development or migration scripts
public async Task ApplySchemaChangesAsync(IServiceProvider services)
{
    // Get catalog document store
    var catalogStore = services.GetRequiredService<IDocumentStore>(); // Resolved with "catalog" name

    // Apply schema changes for catalog context
    await catalogStore.Schema.ApplyAllConfiguredChangesToDatabaseAsync();

    // Repeat for other contexts...
}
```

### Schema Export

Export schema scripts for each context:

```csharp
public void ExportSchemas(IServiceProvider services)
{
    var catalogStore = services.GetRequiredService<IDocumentStore>();

    var script = catalogStore.Schema.ToDDL();
    File.WriteAllText("catalog-schema.sql", script);
}
```

## Testing

### Integration Tests with Bounded Contexts

```csharp
public class BoundedContextIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres;
    private ServiceProvider _serviceProvider;
    private IDbSession<ICatalogContext> _catalogSession;
    private IDbSession<ISalesContext> _salesSession;

    public BoundedContextIntegrationTests()
    {
        _postgres = new PostgreSqlBuilder()
            .WithImage("postgres:15-alpine")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        var services = new ServiceCollection();

        // Register catalog context
        services.AddMarten("catalog", options =>
        {
            options.Connection(_postgres.GetConnectionString());
            options.DatabaseSchemaName = "catalog";
            options.Schema.For<Product>().Identity(x => x.Id);
        });

        // Register sales context
        services.AddMarten("sales", options =>
        {
            options.Connection(_postgres.GetConnectionString());
            options.DatabaseSchemaName = "sales";
            options.Schema.For<Order>().Identity(x => x.Id);
        });

        services.AddMartenExpressions()
            .AddContext<ICatalogContext, IDocumentStore>()
            .AddContext<ISalesContext, IDocumentStore>();

        _serviceProvider = services.BuildServiceProvider();

        // Apply schemas
        var catalogStore = _serviceProvider.GetRequiredService<IDocumentStore>();
        await catalogStore.Schema.ApplyAllConfiguredChangesToDatabaseAsync();

        _catalogSession = _serviceProvider.GetRequiredService<IDbSession<ICatalogContext>>();
        _salesSession = _serviceProvider.GetRequiredService<IDbSession<ISalesContext>>();

        // Seed data
        _catalogSession.Add(new Product { Id = Guid.NewGuid(), Name = "Test Product" });
        await _catalogSession.SaveChangesAsync();

        _salesSession.Add(new Order { Id = Guid.NewGuid(), CustomerId = Guid.NewGuid() });
        await _salesSession.SaveChangesAsync();
    }

    [Fact]
    public async Task Should_Isolate_Contexts()
    {
        // Catalog context has products
        var catalogQuery = _catalogSession.Query<Product>();
        var products = await catalogQuery.ToListAsync();
        Assert.NotEmpty(products);

        // Sales context has orders
        var salesQuery = _salesSession.Query<Order>();
        var orders = await salesQuery.ToListAsync();
        Assert.NotEmpty(orders);
    }

    public async Task DisposeAsync()
    {
        await _serviceProvider.DisposeAsync();
        await _postgres.DisposeAsync();
    }
}
```

## Best Practices

### Do's

✅ **Use separate schemas** - Isolate contexts using PostgreSQL schemas  
✅ **Configure separately** - Each context can have its own document configurations  
✅ **Use event sourcing per context** - Keep event streams isolated  
✅ **Apply schema changes independently** - Each context manages its own schema  
✅ **Test isolation** - Verify contexts don't interfere with each other  

### Don'ts

❌ **Don't share document types** - Each context should own its documents  
❌ **Don't mix schemas** - Keep document types in their respective schemas  
❌ **Don't use distributed transactions** - Prefer eventual consistency  
❌ **Don't couple contexts** - Keep context implementations independent  
❌ **Don't share connection pools unsafely** - Ensure proper isolation  

## Performance Considerations

### Indexing per Context

Create indexes specific to each context's query patterns:

```csharp
services.AddMarten(options =>
{
    options.Connection(connectionString);

    // Catalog context indexes
    options.Schema.For<Product>()
        .Identity(x => x.Id)
        .DatabaseSchemaName("catalog")
        .Index(x => x.Category)
        .Index(x => x.Price)
        .GinIndexJsonData(); // Full-text search on JSON

    // Sales context indexes
    options.Schema.For<Order>()
        .Identity(x => x.Id)
        .DatabaseSchemaName("sales")
        .Index(x => x.CustomerId)
        .Index(x => x.OrderDate);
});
```

### Connection Pooling

Marten uses Npgsql connection pooling automatically. Each context can have independent connection pools:

```csharp
services.AddMarten("catalog", options =>
{
    options.Connection("Host=localhost;Database=catalog;Maximum Pool Size=100;...");
});

services.AddMarten("sales", options =>
{
    options.Connection("Host=localhost;Database=sales;Maximum Pool Size=50;...");
});
```

## See Also

- [Database Session Bounded Contexts](/database-session/bounded-contexts) - General bounded contexts concepts
- [Marten](/marten/) - Marten integration basics
- [Marten Documentation](https://martendb.io/) - Official Marten docs
- [PostgreSQL Schemas](https://www.postgresql.org/docs/current/ddl-schemas.html) - PostgreSQL schema documentation
