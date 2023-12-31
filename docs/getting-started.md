# Getting Started

The Raiqub Expressions was split into 5 packages to better integrate with layered projects:

- [Raiqub.Expressions](https://www.nuget.org/packages/Raiqub.Expressions/)
- [Raiqub.Expressions.Reading](https://www.nuget.org/packages/Raiqub.Expressions.Reading/)
- [Raiqub.Expressions.Writing](https://www.nuget.org/packages/Raiqub.Expressions.Writing/)
- [Raiqub.Expressions.EntityFrameworkCore](https://www.nuget.org/packages/Raiqub.Expressions.EntityFrameworkCore/)
- [Raiqub.Expressions.Marten](https://www.nuget.org/packages/Raiqub.Expressions.Marten/)

## Entity Framework Core

For projects going to use EF Core you need to follow these steps:

1. Add the required NuGet package(s) for the database provider you'll be using, such as _Microsoft.EntityFrameworkCore.SqlServer_:

::: code-group

```shell [.NET CLI]
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

```powershell [Powershell]
PM> Install-Package Microsoft.EntityFrameworkCore.SqlServer
```

```shell [Paket]
paket add nuget Microsoft.EntityFrameworkCore.SqlServer
```

:::

2. Add the _Raiqub.Expressions.EntityFrameworkCore_ library from NuGet:

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

3. Register your DbContext by using _AddDbContextFactory_ extension method (more details on [EF Core documentation](https://learn.microsoft.com/en-us/ef/core/modeling/)):

```csharp
services.AddDbContextFactory<YourDbContext>();
```

1. Register the session and session factories using the following extension method:

```csharp
services.AddEntityFrameworkExpressions()
    .AddSingleContext<YourDbContext>();
```

## Marten

For projects going to use Marten you need to follow these steps:

1. Add the _Raiqub.Expressions.Marten_ library from NuGet:

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

2. Register the session and session factories using the following extension method:

```csharp
services.AddMartenExpressions()
    .AddSingleContext();
```

## Using

Inject the appropriate session interface (`IDbQuerySession` for read sessions, `IDbSession` for read and write sessions) into your services, and use it read and write from/to database.

::: code-group

```csharp [C# classic]
public class YourService
{
    private readonly IDbSession _dbSession;

    public YourService(IDbSession dbSession)
    {
        _dbSession = dbSession;
    }

    // ...
}
```

```csharp [C# 12]
public class YourService(IDbSession dbSession)
{
    // ...
}
```

:::

You can also create specifications and query strategies. Here's an example of how to create a simple specification:

```csharp
public class CustomerIsActive : Specification<Customer>
{
    public override Expression<Func<Customer, bool>> ToExpression()
    {
        return customer => customer.IsActive;
    }
}
```

And here's an example of how to use the specification with database session:

```csharp
// Assuming 'session' is of type IDbSession or IDbQuerySession and has been injected
var query = session.Query(new CustomerIsActive());
var customers = await query.ToListAsync();
```