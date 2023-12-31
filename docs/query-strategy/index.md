# Query Strategy

The query strategy is based on the Strategy Pattern by defining a strategy for querying the database allowing better concern separation, maintainability and reusability than the repository pattern.

The **\`Raiqub.Expressions.Reading\`** package provides abstractions for creating query strategies. You can create a new query strategy by choosing one of several ways available to implement a query strategy.

To add Raiqub.Expressions.Reading library to a .NET project, go get it from Nuget:

::: code-group

```shell [.NET CLI]
dotnet add package Raiqub.Expressions.Reading
```

```powershell [Powershell]
PM> Install-Package Raiqub.Expressions.Reading
```

```shell [Paket]
paket add nuget Raiqub.Expressions.Reading
```

:::

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