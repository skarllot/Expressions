# Raiqub Expressions Reading - Query Strategy and Database Session

_Provides abstractions for creating query strategies, query sessions and querying from database (defines IDbQuerySessionFactory and IDbQuerySession interfaces)_

[![Build status](https://github.com/skarllot/Expressions/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/skarllot/Expressions/actions)
[![NuGet](https://buildstats.info/nuget/Raiqub.Expressions.Reading)](https://www.nuget.org/packages/Raiqub.Expressions.Reading/)
[![Code coverage](https://codecov.io/gh/skarllot/Expressions/branch/main/graph/badge.svg)](https://codecov.io/gh/skarllot/Expressions)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2Fskarllot%2FExpressions%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/skarllot/Expressions/main)
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg?style=flat)](https://raw.githubusercontent.com/skarllot/Expressions/master/LICENSE)

## Documentation and Samples
Documentation, and samples, for using Raiqub Expressions can be found in the repository's [README](https://github.com/skarllot/Expressions#readme) and [documentation](https://fgodoy.me/Expressions/).

## Quick Example

Single entity query:

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

Multiple entities query:

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

Using database session:

```csharp
// Assuming 'session' is of type IDbQuerySession and has been injected
var query = session.Query(new CustomerIsActive());
var customers = await query.ToListAsync();
```

## Release Notes
See [GitHub Releases](https://github.com/skarllot/Expressions/releases) for release notes.
