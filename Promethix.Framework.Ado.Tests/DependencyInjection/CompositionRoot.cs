using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Promethix.Framework.Ado.Enums;
using Promethix.Framework.Ado.Implementation;
using Promethix.Framework.Ado.Interfaces;
using Promethix.Framework.Ado.Tests.TestSupport.DataAccess.Mssql;
using Promethix.Framework.Ado.Tests.TestSupport.DataAccess.Sqlite;
using SQLitePCL;
using System.Data;
using System.Data.Common;


namespace Promethix.Framework.Ado.Tests.DependencyInjection
{
    public static class CompositionRoot
    {
        public static void AddIntegrationDependencyInjectionJsonExample(this IServiceCollection services)
        {
            // Register configuration file
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.example2.json", optional: true, reloadOnChange: true)
                .Build();

            StandardRegistrations(services, configuration);

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
        }

        /// <summary>
        ///  Demonstrates several ways to configure AdoScope and AdoContext with a
        ///  hybrid JSON, Fluent and Constructor driven options.
        /// </summary>
        /// <param name="services"></param>
        public static void AddIntegrationDependencyInjectionHybridExample(this IServiceCollection services)
        {
            // Register configuration file
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.example1.json", optional: true, reloadOnChange: true)
                .Build();

            StandardRegistrations(services, configuration);

            var adoScopeConfiguration = new AdoScopeConfigurationBuilder()
            .ConfigureScope(options => { _ = options.WithScopeExecutionOption(AdoContextGroupExecutionOption.Standard); })
            .Build();

            var adoContextConfiguration = new AdoContextConfigurationBuilder()
                .AddAdoContext<SqliteContextExample1>(options =>
                {
                    // JSON AdoContext Configuration File Example 1
                    _ = options.WithNamedContext("SqliteContextExample1", configuration);
                })
                .AddAdoContext<SqliteContextExample2>(options => 
                {
                    // JSON AdoContext Configuration File Example 2
                    _ = options.WithNamedContext("SqliteContextExample2", configuration);
                })
                .AddAdoContext<SqliteContextExample3>(options =>
                {
                    // JSON AdoContext Configuration File Example 3
                    _ = options.WithNamedContext("SqliteContextExample3", configuration);
                })
                .AddAdoContext<SqliteContextExample4>(options =>
                {
                    // Fluent AdoContext Configuration File Example 4
                    _ = options.WithNamedContext("SqliteContextExample4")
                            .WithConnectionString("Data Source=mydatabase4.db")
                            .WithProviderName("Microsoft.Data.Sqlite")
                            .WithExecutionOption(AdoContextExecutionOption.Transactional)
                            .WithDefaultIsolationLevel(IsolationLevel.ReadCommitted);
                })
                .Build();

            // NOTE: AdoContext SqliteContextExample5 uses constructor configuration and
            // doesn't need to be registered at all.

            // Register entire AdoScope configuration in DI
            _ = services.AddScoped(provider => adoScopeConfiguration);
            _ = services.AddScoped(provider => adoContextConfiguration);
        }

        private static void StandardRegistrations(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration);

            // Still need to register ADO providers you will be using
            DbProviderFactories.RegisterFactory("Microsoft.Data.Sqlite", SqliteFactory.Instance);
            DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance);

            // Register your repositories et al
            _ = services.AddSingleton<IAmbientAdoContextLocator, AmbientAdoContextLocator>();
            _ = services.AddSingleton<IAdoScopeFactory, AdoScopeFactory>();
            _ = services.AddSingleton<IAdoContextGroupFactory, AdoContextGroupFactory>();
            _ = services.AddScoped<ISimpleTestRepository, SimpleTestRepository>();
            _ = services.AddScoped<IMultiTestRepository, MultiTestRepository>();
            _ = services.AddScoped<ISimpleMssqlTestRepository, SimpleMssqlTestRepository>();
        }
    }
}
