# Creating Query Strategies

There are several ways to create query strategies depending on your needs. This guide covers the main approaches.

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

## Value Types Results

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
