using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Promethix.Framework.Ado.Enums;
using Promethix.Framework.Ado.Implementation;
using Promethix.Framework.Ado.Interfaces;
using Promethix.Framework.Ado.Tests.TestSupport.DataAccess;
using SQLitePCL;
using System.Data;
using System.Data.Common;


namespace Promethix.Framework.Ado.Tests.DependencyInjection
{
    public static class CompositionRoot
    {
        public static void AddIntegrationDependencyInjection(this IServiceCollection services)
        {
            // Register configuration file
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            // Still need to register ADO providers you will be using
            DbProviderFactories.RegisterFactory("Microsoft.Data.Sqlite", SqliteFactory.Instance);

            // Register your repositories et al
            _ = services.AddSingleton<IAmbientAdoContextLocator, AmbientAdoContextLocator>();
            _ = services.AddSingleton<IAdoScopeFactory, AdoScopeFactory>();
            _ = services.AddSingleton<IAdoContextGroupFactory, AdoContextGroupFactory>();
            _ = services.AddScoped<ISimpleTestRepository, SimpleTestRepository>();
            _ = services.AddScoped<IMultiTestRepository, MultiTestRepository>();

            // Register your ADO contexts
            var adoContextConfiguration = new AdoContextConfigurationBuilder()
                .AddAdoContext<SqliteContextExample1>(options =>
                {
                    _ = options.WithNamedConnection("SqliteContextExample1", configuration);
                })
                .AddAdoContext<SqliteContextExample2>(options => 
                {
                    _ = options.WithNamedConnection("SqliteContextExample2", configuration);
                })
                .AddAdoContext<SqliteContextExample3>(options =>
                {
                    _ = options.WithNamedConnection("SqliteContextExample3", configuration);
                })
                .AddAdoContext<SqliteContextExample4>(options =>
                {
                    _ = options.WithNamedConnection("SqliteContextExample4");
                    _ = options.WithConnectionString("Data Source=mydatabase4.db");
                    _ = options.WithProviderName("Microsoft.Data.Sqlite");
                    _ = options.WithExecutionOption(AdoContextExecutionOption.Transactional);
                    _ = options.WithDefaultIsolationLevel(IsolationLevel.ReadCommitted);
                })
                .Build();

            _ = services.AddScoped(provider => adoContextConfiguration);  
        }
    }
}
