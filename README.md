# AdoScope (Pre-Release Alpha)

[![Build and Test 0.1.x-alpha](https://github.com/gentoorax/Promethix.Framework.Ado/actions/workflows/adoscope-nuget-build.yaml/badge.svg)](https://github.com/gentoorax/Promethix.Framework.Ado/actions/workflows/adoscope-nuget-build.yaml)
[![Published to nuget.org 0.1.x-x-alpha](https://github.com/gentoorax/Promethix.Framework.Ado/actions/workflows/adoscope-nuget-publish-prerelease.yaml/badge.svg)](https://github.com/gentoorax/Promethix.Framework.Ado/actions/workflows/adoscope-nuget-publish-prerelease.yaml)

AdoScope offers a simple and flexible solution for managing your ADO.NET connections and transactions. It draws inspiration
from the remarkable work in DbContextScope by Mehdime El Gueddari, whose DbContextScope library has been a source of
great inspiration for the creation of AdoScope.

While AdoScope is compatible with any ADO.NET provider, it was specifically designed with Dapper in mind. Having extensive
experience with Entity Framework and DbContextScope, the goal was to provide a similar solution tailored to the requirements
of Dapper.

Unlike Entity Framework, Dapper lacks a DbContext, which can lead to challenges in managing DbConnection and DbTransaction.
To address this, AdoScope introduces the concept of an AdoContext—a wrapper around DbConnection and DbTransaction, simplifying their management.

If you are seeking a Unit of Work pattern for Dapper with minimal coding overhead, AdoScope provides an elegant solution.

## Supported Features

- [x] Simple and flexible configuration
- [x] Database provider agnostic
- [x] Support for nested transactions
- [x] Support for multiple database connections
- [x] Support for explicit database transactions
- [x] Context specific execution options (transactional, non-transactional)
- [x] Support for specific isolation levels per context and per (explicit) transaction
- [x] Support for multiple databases in a single distributed transaction
- [x] Support for explicit distributed transactions
- [x] Support for asynchronous operations
- [x] Support for default distributed transactions

## Future Features

- [ ] Support for read only transactions

## Please be aware

That when your `AdoContext` is configured in transactional mode, it will hold a transaction open until you call
`Complete()` or `Dispose()` on the `AdoScope`, this is by design. If you do not want this behavior, configure your `AdoContext` to be non-transactional.

`CreateWithTransaction()` forces the creation of a new ambient `AdoContext` (i.e. does not join the ambient scope if there is one) and wraps all
`AdoContext` instances created within that scope in an explicit database transaction with the provided isolation level.

`CreateWithDistributedTransaction()` forces the creation of a new ambient `AdoContext` (i.e. does not join the ambient scope if there is one) and wraps all
`AdoContext` instances created within that scope in an *distributed transaction*.

**Distributed Transactions will not work at all on .NET 5 or 6. They are supported on .NET 7 or better and only on Windows AFAIK** this is a limitation of
.NET and it's requirement on OS DTC support e.g. MSDTC. MSDTC will of course need to be enabled and running on Windows as well.

## Usage

Install the NuGet package
```powershell
Install-Package AdoScope -Version 0.1.42-alpha
```

Create an ADO Context
```csharp
public class SqliteContextExample1 : AdoContext
    {
        public SqliteContextExample1()
        {
            // No Implementation
        }
    }
```

Create a Repository making use of this context
```csharp
public class SimpleTestRepository : ISimpleTestRepository
{
    private readonly IAmbientAdoContextLocator ambientAdoContextLocator;

    public SimpleTestRepository(IAmbientAdoContextLocator ambientAdoContextLocator)
    {
        this.ambientAdoContextLocator = ambientAdoContextLocator;
    }

    private IDbConnection SqliteConnection => ambientAdoContextLocator.GetContext<SqliteContextExample1>().Connection;

    public void Add(TestEntity entity)
    {
        const string query = "INSERT INTO TestEntity (Name, Description, Quantity) VALUES (@Name, @Description, @Quantity)";
        SqliteConnection.Execute(query, entity);
    }

    public TestEntity GetEntityByName(string name)
    {
        const string query = "SELECT * FROM TestEntity WHERE Name = @Name";
        return SqliteConnection.QuerySingleOrDefault<TestEntity>(query, new { Name = name });
    }
}
```

Create a Service making use of this repository and AdoScope.

When the ADO Context is configured with a transactional execution option, this will behave as a Unit of Work. It will commit when Complete() is called.

You can also configure the ADO Context to be non-transactional, in which case it will behave as a simple connection manager, executing queries as they are called.
```csharp
public void ServiceLayerAddTestEntity()
{
    using IAdoScope adoScope = adoScopeFactory.Create();

    // Create a test entity
    var newTestEntity = new TestEntity { Name = "CreateTest", Description = "Test Description", Quantity = 1 };

    // Call our repository to add the entity
    simpleTestRepository.Add(newTestEntity);

    // Commit the unit of work / transaction (if using ExecutionOption.Transactional)
    adoScope.Complete();
}
```

Configure your DI Container. Just one of many examples of configuration. This example is
by hand to show the available options. Recommend you use appsettings.json for configuration. 
See example below this one for that.
```csharp
// Still need to register ADO providers you will be using. This is a .NET ADO requirement.
DbProviderFactories.RegisterFactory("Microsoft.Data.Sqlite", SqliteFactory.Instance);

// Register your repositories et al
_ = services.AddSingleton<IAmbientAdoContextLocator, AmbientAdoContextLocator>();
_ = services.AddSingleton<IAdoScopeFactory, AdoScopeFactory>();
_ = services.AddSingleton<IAdoContextGroupFactory, AdoContextGroupFactory>();
_ = services.AddScoped<ISimpleTestRepository, SimpleTestRepository>();
_ = services.AddScoped<IMultiTestRepository, MultiTestRepository>();

// Register your ADO Contexts
var adoContextConfiguration = new AdoContextConfigurationBuilder()
.AddAdoContext<SqliteContextExample1>(options =>
{
    _ = options.WithNamedConnection("SqliteContextExample");
    _ = options.WithConnectionString("Data Source=mydatabase.db");
    _ = options.WithProviderName("Microsoft.Data.Sqlite");
    _ = options.WithExecutionOption(AdoContextExecutionOption.Transactional);
    _ = options.WithDefaultIsolationLevel(IsolationLevel.ReadCommitted);
})
.Build();

_ = services.AddScoped(provider => adoContextConfiguration);  
```

### Recommended Approach

Use appsettings.json for configuration, you can use the following code in your DI composition:
```csharp
// AdoScope Configuration
var adoScopeConfiguration = new AdoScopeConfigurationBuilder()
.ConfigureScope(options =>
{
    _ = options.WithScopeConfiguration(configuration);
})
.Build();

// AdoContexts Configuration
var adoContextConfiguration = new AdoContextConfigurationBuilder()
    .AddAdoContext<SqliteContextExample1>(options =>
    {
        // JSON AdoContext Configuration File Example 1
        _ = options.WithNamedContext("SqliteContextExample1", configuration);
    })
    .AddAdoContext<SqliteContextExample3>(options =>
    {
        // JSON AdoContext Configuration File Example 3
        _ = options.WithNamedContext("SqliteContextExample3", configuration);
    })
    .Build();

// Register entire AdoScope configuration in DI
_ = services.AddScoped(provider => adoScopeConfiguration);
_ = services.AddScoped(provider => adoContextConfiguration); 
```

For most use cases you want the following configuration:
- Distributed Transactions Off.
- AdoContext Transactions On - provides Unit of Work behaviour.

appsettings.json as follows (see unit test project for more examples):
```json
{
  "AdoScopeOptions": {
    "ScopeExecutionOption": "Standard",
  },
  "AdoContextOptions": {
    "SqliteContextExample1": {
      "ProviderName": "Microsoft.Data.Sqlite",
      "ConnectionString": "Data Source=mydatabase.db",
      "ExecutionOption": "Transactional"
    },
    "SqliteContextExample3": {
      "ProviderName": "Microsoft.Data.Sqlite",
      "ConnectionString": "Data Source=mydatabase3.db",
      "ExecutionOption": "Transactional"
    }
  }
}
```

For MS SQL Server the Provider Name should be `System.Data.SqlClient`