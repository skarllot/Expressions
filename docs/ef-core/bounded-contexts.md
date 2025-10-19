# Entity Framework Core Bounded Contexts

This guide explains how to configure and use bounded contexts with Entity Framework Core in Raiqub Expressions.

## Prerequisites

Before working with bounded contexts, ensure you have:

1. **Entity Framework Core** configured in your project
2. **Raiqub.Expressions.EntityFrameworkCore** package installed
3. DbContext classes defined for your contexts

## Registering Bounded Contexts

### Single DbContext per Context

The most common approach is to have separate DbContext classes for each bounded context:

```csharp
// Define bounded context interfaces
public interface ICatalogContext { }
public interface ISalesContext { }
public interface IReportingContext { }

// Define DbContext classes
public class CatalogDbContext : DbContext, ICatalogContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
}

public class SalesDbContext : DbContext, ISalesContext
{
    public SalesDbContext(DbContextOptions<SalesDbContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<Customer> Customers { get; set; }
}

public class ReportingDbContext : DbContext, IReportingContext
{
    public ReportingDbContext(DbContextOptions<ReportingDbContext> options)
        : base(options)
    {
    }

    // Read-only views or entities for reporting
    public DbSet<OrderSummary> OrderSummaries { get; set; }
}
```

### Register with Dependency Injection

```csharp
// Register DbContext factories
services.AddDbContextFactory<CatalogDbContext>(options =>
    options.UseSqlServer(catalogConnectionString));

services.AddDbContextFactory<SalesDbContext>(options =>
    options.UseSqlServer(salesConnectionString));

services.AddDbContextFactory<ReportingDbContext>(options =>
    options.UseSqlServer(reportingConnectionString)
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

// Register bounded context sessions
services.AddEntityFrameworkExpressions()
    .AddContext<ICatalogContext, CatalogDbContext>()
    .AddContext<ISalesContext, SalesDbContext>()
    .AddContext<IReportingContext, ReportingDbContext>(ChangeTracking.Disabled);
```

### Multiple Contexts Sharing Single DbContext

In some scenarios, you might want multiple logical contexts sharing a single DbContext:

```csharp
public class ApplicationDbContext : DbContext,
    ICatalogContext,
    ISalesContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Catalog entities
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    // Sales entities
    public DbSet<Order> Orders { get; set; }
    public DbSet<Customer> Customers { get; set; }
}

// Register
services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

services.AddEntityFrameworkExpressions()
    .AddContext<ICatalogContext, ApplicationDbContext>()
    .AddContext<ISalesContext, ApplicationDbContext>();
```

## Configuration Options

### Change Tracking

Configure change tracking behavior per context:

```csharp
services.AddEntityFrameworkExpressions()
    // Write context with tracking enabled
    .AddContext<ICatalogContext, CatalogDbContext>(ChangeTracking.Enabled)
    // Read-only context with tracking disabled
    .AddContext<IReportingContext, ReportingDbContext>(ChangeTracking.Disabled);
```

### Query Configuration

Configure query-specific options per entity type within a context:

```csharp
services.AddEntityFrameworkExpressions()
    .Configure<Product>(options =>
    {
        options.UseSplitQuery = true;  // Use split queries for Product
    })
    .Configure<Order>(options =>
    {
        options.UseSplitQuery = true;  // Use split queries for Order
    })
    .AddContext<ICatalogContext, CatalogDbContext>()
    .AddContext<ISalesContext, SalesDbContext>();
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

## Connection Strings and Database Separation

### Separate Databases

Use different connection strings for complete database isolation:

```csharp
services.AddDbContextFactory<CatalogDbContext>(options =>
    options.UseSqlServer("Server=localhost;Database=Catalog;..."));

services.AddDbContextFactory<SalesDbContext>(options =>
    options.UseSqlServer("Server=localhost;Database=Sales;..."));

services.AddEntityFrameworkExpressions()
    .AddContext<ICatalogContext, CatalogDbContext>()
    .AddContext<ISalesContext, SalesDbContext>();
```

### Same Database, Different Schemas

Use database schemas to separate contexts within a single database:

```csharp
public class CatalogDbContext : DbContext, ICatalogContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("catalog");

        modelBuilder.Entity<Product>().ToTable("Products", "catalog");
        modelBuilder.Entity<Category>().ToTable("Categories", "catalog");
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
}

public class SalesDbContext : DbContext, ISalesContext
{
    public SalesDbContext(DbContextOptions<SalesDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("sales");

        modelBuilder.Entity<Order>().ToTable("Orders", "sales");
        modelBuilder.Entity<Customer>().ToTable("Customers", "sales");
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<Customer> Customers { get; set; }
}

// Both use same connection string but different schemas
var connectionString = "Server=localhost;Database=MyApp;...";

services.AddDbContextFactory<CatalogDbContext>(options =>
    options.UseSqlServer(connectionString));

services.AddDbContextFactory<SalesDbContext>(options =>
    options.UseSqlServer(connectionString));
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

### Transaction Considerations

**Important:** Each DbContext has its own transaction scope. For operations spanning multiple contexts:

```csharp
public class CrossContextService
{
    private readonly IDbSession<ICatalogContext> _catalogSession;
    private readonly IDbSession<ISalesContext> _salesSession;

    public CrossContextService(
        IDbSession<ICatalogContext> catalogSession,
        IDbSession<ISalesContext> salesSession)
    {
        _catalogSession = catalogSession;
        _salesSession = salesSession;
    }

    public async Task ProcessOrderWithInventoryAsync(
        Order order,
        CancellationToken cancellationToken = default)
    {
        // Note: These are separate transactions!
        // Consider eventual consistency patterns instead of distributed transactions

        try
        {
            // Update inventory first
            foreach (var item in order.Items)
            {
                var productQuery = _catalogSession.Query<Product>()
                    .Where(p => p.Id == item.ProductId);
                var product = await productQuery.FirstAsync(cancellationToken);

                product.AvailableQuantity -= item.Quantity;
                _catalogSession.Update(product);
            }

            await _catalogSession.SaveChangesAsync(cancellationToken);

            // Then save order
            _salesSession.Add(order);
            await _salesSession.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // Implement compensation logic or use Saga pattern
            throw;
        }
    }
}
```

## Migrations

Manage migrations separately for each bounded context:

```bash
# Create migration for catalog context
dotnet ef migrations add InitialCreate --context CatalogDbContext --output-dir Migrations/Catalog

# Create migration for sales context
dotnet ef migrations add InitialCreate --context SalesDbContext --output-dir Migrations/Sales

# Apply migrations
dotnet ef database update --context CatalogDbContext
dotnet ef database update --context SalesDbContext
```

## Testing

### Integration Tests with Bounded Contexts

```csharp
public class BoundedContextIntegrationTests : IAsyncLifetime
{
    private readonly ServiceProvider _serviceProvider;
    private readonly IDbSession<ICatalogContext> _catalogSession;
    private readonly IDbSession<ISalesContext> _salesSession;

    public BoundedContextIntegrationTests()
    {
        var services = new ServiceCollection();

        // Register test databases
        services.AddDbContextFactory<CatalogDbContext>(options =>
            options.UseInMemoryDatabase("CatalogTest"));

        services.AddDbContextFactory<SalesDbContext>(options =>
            options.UseInMemoryDatabase("SalesTest"));

        services.AddEntityFrameworkExpressions()
            .AddContext<ICatalogContext, CatalogDbContext>()
            .AddContext<ISalesContext, SalesDbContext>();

        _serviceProvider = services.BuildServiceProvider();
        _catalogSession = _serviceProvider.GetRequiredService<IDbSession<ICatalogContext>>();
        _salesSession = _serviceProvider.GetRequiredService<IDbSession<ISalesContext>>();
    }

    public async Task InitializeAsync()
    {
        // Seed catalog context
        _catalogSession.Add(new Product { Id = Guid.NewGuid(), Name = "Test Product" });
        await _catalogSession.SaveChangesAsync();

        // Seed sales context
        _salesSession.Add(new Customer { Id = Guid.NewGuid(), Name = "Test Customer" });
        await _salesSession.SaveChangesAsync();
    }

    [Fact]
    public async Task Should_Isolate_Contexts()
    {
        // Catalog context has products
        var catalogQuery = _catalogSession.Query<Product>();
        var products = await catalogQuery.ToListAsync();
        Assert.NotEmpty(products);

        // Sales context has customers
        var salesQuery = _salesSession.Query<Customer>();
        var customers = await salesQuery.ToListAsync();
        Assert.NotEmpty(customers);
    }

    public async Task DisposeAsync()
    {
        await _serviceProvider.DisposeAsync();
    }
}
```

## Best Practices

### Do's

✅ **Use separate DbContexts** - One DbContext per bounded context  
✅ **Configure separately** - Each context can have its own configuration  
✅ **Use schemas** - Separate contexts using database schemas  
✅ **Manage migrations separately** - Each context has independent migrations  
✅ **Test isolation** - Verify contexts don't interfere with each other  

### Don'ts

❌ **Don't share DbContext** - Avoid using a single DbContext for unrelated contexts  
❌ **Don't use distributed transactions** - Prefer eventual consistency  
❌ **Don't couple contexts** - Keep context implementations independent  
❌ **Don't share connection strings** - Unless using schema separation  
❌ **Don't mix entity definitions** - Each context owns its entities  

## See Also

- [Database Session Bounded Contexts](/database-session/bounded-contexts) - General bounded contexts concepts
- [Entity Framework Core](/ef-core/) - EF Core integration basics
- [Custom SQL Query](/ef-core/custom-sql) - Custom SQL in EF Core
- [Split Query](/ef-core/split-query) - Split query configuration
- [EF Core Documentation](https://learn.microsoft.com/en-us/ef/core/) - Official EF Core docs
