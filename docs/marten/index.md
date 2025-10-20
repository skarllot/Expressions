# Marten

The implementation of database sessions using Marten provides configuration and registration for integrating with the [Marten](https://martendb.io/) document database. Marten uses PostgreSQL as a document database and event store for .NET applications.

To add Raiqub.Expressions.Marten library to a .NET project, go get it from Nuget:

::: code-group

```shell [.NET CLI]
dotnet add package Raiqub.Expressions.Marten
```

```powershell [Powershell]
PM> Install-Package Raiqub.Expressions.Marten
```

```shell [Paket]
paket add nuget Raiqub.Expressions.Marten
```

:::

## Prerequisites

Before using Raiqub.Expressions.Marten, ensure you have Marten configured in your project. Add the Marten package:

```shell
dotnet add package Marten
```

## Marten Configuration

First, configure Marten with your PostgreSQL connection string and document mappings:

```csharp
services.AddMarten(options =>
{
    // Configure connection string
    options.Connection("Host=localhost;Database=myapp;Username=postgres;Password=password");

    // Configure document mappings
    options.Schema.For<Product>().Identity(x => x.Id);
    options.Schema.For<Order>().Identity(x => x.Id);
    options.Schema.For<Customer>().Identity(x => x.Id);

    // Optional: Configure JSON serialization
    options.UseDefaultSerialization(
        serializerType: SerializerType.SystemTextJson,
        enumStorage: EnumStorage.AsString);
});
```

## Register Database Session

After configuring Marten, register the session and session factories using the following extension method:

```csharp
services.AddMartenExpressions()
    .AddSingleContext();
```

This registers:
- `IDbQuerySessionFactory` - For creating read-only query sessions
- `IDbSessionFactory` - For creating read/write sessions
- `IDbQuerySession` - For querying documents
- `IDbSession` - For querying and modifying documents

## Basic Usage

### Querying Documents

Use specifications and query strategies with Marten just like with Entity Framework Core:

```csharp
public class ProductService
{
    private readonly IDbQuerySession _session;

    public ProductService(IDbQuerySession session)
    {
        _session = session;
    }

    public async Task<IReadOnlyList<Product>> GetInStockProductsAsync(
        CancellationToken cancellationToken = default)
    {
        var query = _session.Query(ProductSpecification.IsInStock);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<Product>> GetProductsPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _session.Query<Product>();
        return await query.ToPagedListAsync(pageNumber, pageSize, cancellationToken);
    }
}
```

### Persisting Documents

Use the write session to add, update, or delete documents:

```csharp
public class ProductService
{
    private readonly IDbSession _session;

    public ProductService(IDbSession session)
    {
        _session = session;
    }

    public async Task CreateProductAsync(Product product, CancellationToken cancellationToken = default)
    {
        _session.Add(product);
        await _session.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateProductAsync(Product product, CancellationToken cancellationToken = default)
    {
        _session.Update(product);
        await _session.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteProductAsync(Product product, CancellationToken cancellationToken = default)
    {
        _session.Remove(product);
        await _session.SaveChangesAsync(cancellationToken);
    }
}
```

## Query Strategies with Marten

Query strategies work seamlessly with Marten's LINQ provider:

```csharp
public class GetActiveCustomersQueryStrategy : EntityQueryStrategy<Customer, CustomerDto>
{
    protected override IQueryable<CustomerDto> ExecuteCore(IQueryable<Customer> source)
    {
        return source
            .Where(c => c.IsActive)
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .Select(c => new CustomerDto
            {
                Id = c.Id,
                FullName = $"{c.FirstName} {c.LastName}",
                Email = c.Email
            });
    }
}

// Usage
await using var session = sessionFactory.Create();
var query = session.Query(new GetActiveCustomersQueryStrategy());
var customers = await query.ToListAsync();
```

## Marten-Specific Features

### Compiled Queries

Marten supports compiled queries for improved performance. While Raiqub.Expressions doesn't directly expose this, you can create optimized query strategies:

```csharp
public static class ProductQueryStrategy
{
    public static IEntityQueryStrategy<Product, Product> GetByCategory(string category) =>
        QueryStrategy.CreateForEntity(
            (IQueryable<Product> source) => source
                .Where(p => p.Category == category)
                .OrderBy(p => p.Name));
}
```

### Full-Text Search

Marten provides full-text search capabilities that you can use in query strategies:

```csharp
public class SearchProductsByTextQueryStrategy : EntityQueryStrategy<Product, Product>
{
    private readonly string _searchTerm;

    public SearchProductsByTextQueryStrategy(string searchTerm)
    {
        _searchTerm = searchTerm;
    }

    protected override IQueryable<Product> ExecuteCore(IQueryable<Product> source)
    {
        // Marten's PlainTextSearch extension method
        return source.Where(x => x.Search(_searchTerm));
    }
}
```

### Soft Deletes

If you configure soft deletes in Marten, you can create specifications to filter them:

```csharp
public static class ProductSpecification
{
    public static Specification<Product> IsNotDeleted { get; } =
        Specification.Create<Product>(p => !p.IsDeleted || p.Deleted == null);

    public static Specification<Product> IsActive { get; } =
        IsNotDeleted.And(Specification.Create<Product>(p => p.IsActive));
}
```

### Include Related Documents

Marten doesn't use navigation properties like EF Core, but you can load related documents:

```csharp
public class GetOrdersWithCustomerQueryStrategy : IQueryStrategy<OrderWithCustomer>
{
    public IQueryable<OrderWithCustomer> Execute(IQuerySource source)
    {
        var orders = source.GetSet<Order>();
        var customers = source.GetSet<Customer>();

        return from order in orders
               join customer in customers on order.CustomerId equals customer.Id
               select new OrderWithCustomer
               {
                   OrderId = order.Id,
                   OrderDate = order.Date,
                   CustomerName = customer.Name,
                   TotalAmount = order.TotalAmount
               };
    }
}
```

## Advanced Configuration

### Optimistic Concurrency

Marten supports optimistic concurrency. Configure it in your document mappings:

```csharp
services.AddMarten(options =>
{
    options.Connection(connectionString);

    options.Schema.For<Product>()
        .Identity(x => x.Id)
        .UseOptimisticConcurrency(true);
});
```

### Custom Schema

Configure custom database schema for your documents:

```csharp
services.AddMarten(options =>
{
    options.Connection(connectionString);

    // Use custom schema instead of default 'public'
    options.DatabaseSchemaName = "myapp";

    options.Schema.For<Product>().DatabaseSchemaName("catalog");
    options.Schema.For<Order>().DatabaseSchemaName("sales");
});
```

### Indexing

Add indexes to improve query performance:

```csharp
services.AddMarten(options =>
{
    options.Connection(connectionString);

    options.Schema.For<Product>()
        .Identity(x => x.Id)
        .Index(x => x.Category)
        .Index(x => x.Price)
        .Index(x => x.CreatedAt);
});
```

## Performance Tips

### Use Projections

Always project to DTOs to reduce the amount of data transferred:

```csharp
// Good - Only select needed fields
public class GetProductSummaryQueryStrategy : EntityQueryStrategy<Product, ProductSummary>
{
    protected override IQueryable<ProductSummary> ExecuteCore(IQueryable<Product> source)
    {
        return source.Select(p => new ProductSummary
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price
        });
    }
}
```

### Batch Operations

Use batch operations when inserting or updating multiple documents:

```csharp
public async Task CreateProductsAsync(
    IEnumerable<Product> products,
    CancellationToken cancellationToken = default)
{
    foreach (var product in products)
    {
        _session.Add(product);
    }

    // Single SaveChanges for all products
    await _session.SaveChangesAsync(cancellationToken);
}
```

### Streaming Large Result Sets

For large result sets, use `ToAsyncEnumerable` to stream results:

```csharp
public async Task ProcessAllProductsAsync(CancellationToken cancellationToken = default)
{
    var query = _session.Query<Product>();

    await foreach (var product in query.ToAsyncEnumerable(cancellationToken))
    {
        // Process one product at a time
        await ProcessProductAsync(product, cancellationToken);
    }
}
```

## Differences from Entity Framework Core

Key differences when using Marten vs EF Core:

1. **Document Model**: Marten stores documents as JSON, not relational tables
2. **No Foreign Keys**: Relationships are managed in application code
3. **No Migrations**: Schema changes are applied automatically
4. **Event Sourcing**: Marten has built-in event sourcing capabilities
5. **Full-Text Search**: Native PostgreSQL full-text search support
6. **Performance**: Generally faster for read-heavy workloads

## Troubleshooting

### Connection Issues

Ensure your PostgreSQL server is running and accessible:

```csharp
services.AddMarten(options =>
{
    options.Connection("Host=localhost;Port=5432;Database=myapp;Username=postgres;Password=password");

    // Enable detailed logging
    options.Logger(new ConsoleMartenLogger());
});
```

### Schema Updates

If you modify document structure, Marten can update the schema automatically:

```csharp
// In development
var store = services.BuildServiceProvider().GetRequiredService<IDocumentStore>();
await store.Schema.ApplyAllConfiguredChangesToDatabaseAsync();
```

For more information, refer to the official [Marten documentation](https://martendb.io/).