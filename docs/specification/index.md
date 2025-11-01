# Specification

The Specification Pattern is a behavioral design pattern used to encapsulate business rules into composable, reusable and testable objects. This pattern is often used in domains where queries or validation rules need to be expressed in a more readable and maintainable form.

A specification, in the context of this package, is an object that defines a condition that must be satisfied by elements of a certain type. These conditions can be as simple or as complex as needed and are expressed using lambda expressions.

The **`Raiqub.Expressions`** package provides the `Specification<T>` base class for creating specifications. It is optimized to allow ORM frameworks to evaluate and translate it into SQL queries.

## Installation

To add Raiqub.Expressions library to a .NET project, go get it from Nuget:

::: code-group

```shell [.NET CLI]
dotnet add package Raiqub.Expressions
```

```powershell [Powershell]
PM> Install-Package Raiqub.Expressions
```

```shell [Paket]
paket add nuget Raiqub.Expressions
```

:::
