# Query Strategy

The query strategy is based on the Strategy Pattern by defining a strategy for querying the database allowing better concern separation, maintainability and reusability than the repository pattern.

The **`Raiqub.Expressions.Reading`** package provides abstractions for creating query strategies. You can create a new query strategy by choosing one of several ways available to implement a query strategy.

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
