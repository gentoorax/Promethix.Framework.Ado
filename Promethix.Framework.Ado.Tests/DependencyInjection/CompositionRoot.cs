using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Promethix.Framework.Ado.Implementation;
using Promethix.Framework.Ado.Interfaces;
using Promethix.Framework.Ado.Tests.TestSupport.DataAccess;
using System.Data.Common;


namespace Promethix.Framework.Ado.Tests.DependencyInjection
{
    public static class CompositionRoot
    {
        public static void AddIntegrationDependencyInjection(this IServiceCollection services)
        {
            DbProviderFactories.RegisterFactory("Microsoft.Data.Sqlite", SqliteFactory.Instance);
            _ = services.AddSingleton<IAmbientAdoContextLocator, AmbientAdoContextLocator>();
            _ = services.AddSingleton<IAdoScopeFactory, AdoScopeFactory>();
            _ = services.AddScoped<ITestRepository, TestRepository>();
        }
    }
}
