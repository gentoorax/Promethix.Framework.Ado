# AdoScope

[![Build and Test](https://github.com/gentoorax/Promethix.Framework.Ado/actions/workflows/adoscope-nuget-build.yaml/badge.svg)](https://github.com/gentoorax/Promethix.Framework.Ado/actions/workflows/adoscope-nuget-build.yaml)
[![Published to NuGet](https://github.com/gentoorax/Promethix.Framework.Ado/actions/workflows/adoscope-nuget-publish-prerelease.yaml/badge.svg)](https://github.com/gentoorax/Promethix.Framework.Ado/actions/workflows/adoscope-nuget-publish-prerelease.yaml)

---

## 🚀 What is AdoScope?

**AdoScope** is a lightweight and flexible library for Dapper and ADO.NET that manages DbConnection and DbTransaction lifecycles, while providing clean, scoped transactions through an ambient context pattern.

It provides a **minimal-effort Unit of Work pattern**, inspired by [DbContextScope](https://github.com/mehdime/DbContextScope) for Entity Framework — but tailored for Dapper and raw ADO.NET.

It has been used in enterprise-scale applications and production systems, giving it a thorough shake-down in complex, high-volume scenarios.

No need to manually manage transactions, connection lifetimes, or implement repetitive unit of work classes. AdoScope wraps all of this with clean Dependency Injection (DI) support and allows:

- Transparent ambient scoping (no passing around context)
- Cross-database transaction coordination
- **Distributed transactions** (via MSDTC on supported platforms)
- Fine-grained **control per scope or per context**

---

## ✨ Features

- ✅ Lightweight ambient scope for ADO.NET
- ✅ Provider-agnostic (works with MSSQL, SQLite, PostgreSQL, etc.)
- ✅ Minimal effort Unit of Work via scoped transactions
- ✅ Nested scope support
- ✅ Support for **multiple databases** in the same transaction
- ✅ **Distributed transaction** support across databases
- ✅ Asynchronous-safe usage
- ✅ Configurable per context and per scope (transactional / non-transactional)
- ✅ Isolation level configuration per context or scope

---

## 📦 Installation

```powershell
Install-Package Promethix.Framework.Ado
```

---

## 🔧 Configuration (Dependency Injection)

### .NET with SQLite (Appsettings Example)

```csharp
DbProviderFactories.RegisterFactory("Microsoft.Data.Sqlite", SqliteFactory.Instance);

services.AddSingleton<IAmbientAdoContextLocator, AmbientAdoContextLocator>();
services.AddSingleton<IAdoScopeFactory, AdoScopeFactory>();
services.AddSingleton<IAdoContextGroupFactory, AdoContextGroupFactory>();

IConfigurationRoot configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var adoScopeConfig = new AdoScopeConfigurationBuilder()
    .ConfigureScope(options => options.WithScopeConfiguration(configuration))
    .Build();

var adoContextConfig = new AdoContextConfigurationBuilder()
    .AddAdoContext<SqliteDbContext>(options =>
        options.WithNamedContext("SqliteDbContext", configuration))
    .Build();

services.AddScoped(_ => adoScopeConfig);
services.AddScoped(_ => adoContextConfig);
```

### .NET with MSSQL (Appsettings Example)

```csharp
DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance);

services.AddSingleton<IAmbientAdoContextLocator, AmbientAdoContextLocator>();
services.AddSingleton<IAdoScopeFactory, AdoScopeFactory>();
services.AddSingleton<IAdoContextGroupFactory, AdoContextGroupFactory>();

IConfigurationRoot configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var adoScopeConfig = new AdoScopeConfigurationBuilder()
    .ConfigureScope(options => options.WithScopeConfiguration(configuration))
    .Build();

var adoContextConfig = new AdoContextConfigurationBuilder()
    .AddAdoContext<MyDbContext>(options =>
        options.WithNamedContext("MyDbContext", configuration))
    .Build();

services.AddScoped(_ => adoScopeConfig);
services.AddScoped(_ => adoContextConfig);
```

### appsettings.json Example

```json
{
  "AdoScopeOptions": {
    "ScopeExecutionOption": "Standard"
  },
  "AdoContextOptions": {
    "SqliteDbContext": {
      "ProviderName": "Microsoft.Data.Sqlite",
      "ConnectionString": "Data Source=mydatabase.db",
      "ExecutionOption": "Transactional"
    },
    "MyDbContext": {
      "ProviderName": "Microsoft.Data.SqlClient",
      "ConnectionString": "Server=localhost;Database=MyDb;Trusted_Connection=True;",
      "ExecutionOption": "Transactional"
    }
  }
}
```

### .NET Framework Example (Ninject + Fluent Configuration)

```csharp
// Register AdoScope services
kernel.Bind<IAmbientAdoContextLocator>().To<AmbientAdoContextLocator>().InSingletonScope();
kernel.Bind<IAdoScopeFactory>().To<AdoScopeFactory>().InSingletonScope();
kernel.Bind<IAdoContextGroupFactory>().To<AdoContextGroupFactory>().InSingletonScope();

// Register the ADO.NET provider for MSSQL
DbProviderFactory sqlFactory = DbProviderFactories.GetFactory("Microsoft.Data.SqlClient");
kernel.Bind<DbProviderFactory>().ToConstant(sqlFactory);

// Configure AdoScope globally (application-wide scope behavior)
var adoScopeConfig = new AdoScopeConfigurationBuilder()
    .ConfigureScope(options =>
    {
        options.WithScopeExecutionOption(AdoContextGroupExecutionOption.Standard);
    })
    .Build();

// Configure multiple AdoContexts using fluent API
var adoContextConfig = new AdoContextConfigurationBuilder()
    .AddAdoContext<PrimaryDbContext>(options =>
    {
        options.WithNamedContext("PrimaryDbContext")
            .WithConnectionString(ConfigurationManager.ConnectionStrings["PrimaryDb"].ConnectionString)
            .WithProviderName("Microsoft.Data.SqlClient")
            .WithExecutionOption(AdoContextExecutionOption.Transactional)
            .WithDefaultIsolationLevel(IsolationLevel.ReadCommitted);
    })
    .AddAdoContext<AuditDbContext>(options =>
    {
        options.WithNamedContext("AuditDbContext")
            .WithConnectionString(ConfigurationManager.ConnectionStrings["AuditDb"].ConnectionString)
            .WithProviderName("Microsoft.Data.SqlClient")
            .WithExecutionOption(AdoContextExecutionOption.Transactional)
            .WithDefaultIsolationLevel(IsolationLevel.ReadCommitted);
    })
    .Build();

// Register configurations into DI container
kernel.Bind<AdoScopeOptionsBuilder>().ToConstant(adoScopeConfig).InRequestScope();
kernel.Bind<IAdoContextOptionsRegistry>().ToConstant(adoContextConfig).InRequestScope();

```
---

## 🧪 Usage

### 1. Define Your Context

```csharp
public class MyDbContext : AdoContext { }
public class SqliteDbContext : AdoContext { }
```

---

### 2. Create a Repository

```csharp
public class MyRepository : IMyRepository
{
    private readonly IAmbientAdoContextLocator locator;

    public MyRepository(IAmbientAdoContextLocator locator)
    {
        this.locator = locator;
    }

    private IDbConnection Connection => locator.GetContext<MyDbContext>().Connection;
    private IDbTransaction Transaction => locator.GetContext<MyDbContext>().Transaction;

    public int UpdateSomething(string code)
    {
        const string sql = "UPDATE MyTable SET Processed = 1 WHERE Code = @Code";
        return Connection.Execute(sql, new { Code = code }, Transaction);
    }
}
```

---

### 3. Use AdoScope in a Service

```csharp
public class MyService(IAdoScopeFactory adoScopeFactory, IMyRepository repository) : IMyService
{
    public int ProcessItem(string code)
    {
        using IAdoScope adoScope = adoScopeFactory.Create();
        int affected = repository.UpdateSomething(code);
        adoScope.Complete();
        return affected;
    }
}
```

---

### 4. Unit of Work Style Usage with Multiple Repositories

```csharp
using IAdoScope scope = adoScopeFactory.Create();

repository1.DoSomething();
repository2.DoSomethingElse();
repository3.BulkInsert(records);
repository4.MarkAsProcessed(ids);

scope.Complete();
```

---

## ⚠️ Notes and Gotchas

- ✅ You **must call `.Complete()`** to commit a transactional scope. If not called, the transaction will automatically roll back on dispose.
- ✅ If you forget to declare an ambient scope (i.e. no `adoScopeFactory.Create()`), an explicit and helpful exception will be thrown.
- ✅ `CreateWithTransaction()` and `CreateWithDistributedTransaction()` allow full per-scope control of transaction behavior.
- ✅ When using **Distributed Transactions**, ensure:
  - .NET 7+ is used
  - Windows OS with **MSDTC** service is enabled and running
  - ADO.NET provider supports distributed transactions

---

## 🔄 Release Notes

### Stable Releases
- Support for nested scopes, async operations, multiple DBs, and distributed transactions
- Latest release supports .NET 8

---

## 🛣️ Roadmap / Future Features

- [ ] Support for read-only transactions

---

## ❤️ Credits

Inspired by Mehdime El Gueddari’s [DbContextScope](https://github.com/mehdime/DbContextScope) project.

---

## 🙌 Contributing

Pull requests and suggestions welcome! Feel free to open an issue for bugs, ideas, or questions.

---

## 📄 License

MIT
