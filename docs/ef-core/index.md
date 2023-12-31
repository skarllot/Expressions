# Entity Framework Core

The implementation of database sessions using Entity Framework Core provides configuration and registration for using EF.

To add Raiqub.Expressions.EntityFrameworkCore library to a .NET project, go get it from Nuget:

::: code-group

```shell [.NET CLI]
dotnet add package Raiqub.Expressions.EntityFrameworkCore
```

```powershell [Powershell]
PM> Install-Package Raiqub.Expressions.EntityFrameworkCore
```

```shell [Paket]
paket add nuget Raiqub.Expressions.EntityFrameworkCore
```

:::

## DbContext Registration

You need to register the factory of your DbContext by using _AddDbContextFactory_ extension method:

```csharp
services.AddDbContextFactory<YourDbContext>();
```

::: warning
Registering using `AddDbContext` method does not work and will prevent the creation of database sessions.
:::

## Register Database Session

Register the session and session factories using the following extension method:

```csharp
services.AddEntityFrameworkExpressions()
    .AddSingleContext<YourDbContext>();
```