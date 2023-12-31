# Raiqub Expressions - Marten

_Provides implementation of sessions and factories using Marten library_

[![Build status](https://github.com/skarllot/Expressions/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/skarllot/Expressions/actions)
[![NuGet](https://buildstats.info/nuget/Raiqub.Expressions.Marten)](https://www.nuget.org/packages/Raiqub.Expressions.Marten/)
[![Code coverage](https://codecov.io/gh/skarllot/Expressions/branch/main/graph/badge.svg)](https://codecov.io/gh/skarllot/Expressions)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2Fskarllot%2FExpressions%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/skarllot/Expressions/main)
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg?style=flat)](https://raw.githubusercontent.com/skarllot/Expressions/master/LICENSE)

## Documentation and Samples
Documentation, and samples, for using Raiqub Expressions can be found in the repository's [README](https://github.com/skarllot/Expressions#readme) and [documentation](https://fgodoy.me/Expressions/).

## Quick Example

Register database session:

```csharp
services.AddMartenExpressions()
    .AddSingleContext();
```

## Release Notes
See [GitHub Releases](https://github.com/skarllot/Expressions/releases) for release notes.
