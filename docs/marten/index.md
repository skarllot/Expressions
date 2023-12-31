# Marten

The implementation of database sessions using Marten provides configuration and registration for using Marten.

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

## Register Database Session

Register the session and session factories using the following extension method:

```csharp
services.AddMartenExpressions()
    .AddSingleContext();
```