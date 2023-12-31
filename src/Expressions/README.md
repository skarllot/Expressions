# Raiqub Expressions - Specifications

_Provides base for specifications using expression trees that work with repositories_

[![Build status](https://github.com/skarllot/Expressions/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/skarllot/Expressions/actions)
[![NuGet](https://buildstats.info/nuget/Raiqub.Expressions)](https://www.nuget.org/packages/Raiqub.Expressions/)
[![Code coverage](https://codecov.io/gh/skarllot/Expressions/branch/main/graph/badge.svg)](https://codecov.io/gh/skarllot/Expressions)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2Fskarllot%2FExpressions%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/skarllot/Expressions/main)
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg?style=flat)](https://raw.githubusercontent.com/skarllot/Expressions/master/LICENSE)

## Documentation and Samples
Documentation, and samples, for using Raiqub Expressions can be found in the repository's [README](https://github.com/skarllot/Expressions#readme) and [documentation](https://fgodoy.me/Expressions/).

## Quick Example

Simple specification:

```csharp
public class CustomerIsActive : Specification<Customer>
{
    public override Expression<Func<Customer, bool>> ToExpression()
    {
        return customer => customer.IsActive;
    }
}
```

Specification factory:

```csharp
public static class ProductSpecification
{
    public static Specification<Product> IsInStock { get; } =
        Specification.Create<Product>(product => product.AvailableQuantity > 0);

    public static Specification<Product> IsDiscountAvailable(DateTimeOffset now) =>
        Specification.Create<Product>(product => product.DiscountStartDate <= now && now <= product.DiscountEndDate);
}
```

## Release Notes
See [GitHub Releases](https://github.com/skarllot/Expressions/releases) for release notes.
