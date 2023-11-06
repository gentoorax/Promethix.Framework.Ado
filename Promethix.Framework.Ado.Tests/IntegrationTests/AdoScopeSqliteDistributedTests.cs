using Microsoft.Extensions.DependencyInjection;
using Promethix.Framework.Ado.Interfaces;
using Promethix.Framework.Ado.Tests.DependencyInjection;
using Promethix.Framework.Ado.Tests.TestSupport.DataAccess.Sqlite;
using Promethix.Framework.Ado.Tests.TestSupport.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Tests.IntegrationTests
{
    [TestClass]
    public class AdoScopeSqliteImplicitDistributedTests
    {
        private readonly ISimpleTestRepository simpleTestRepository;

        private readonly IAdoScopeFactory adoScopeFactory;

        public AdoScopeSqliteImplicitDistributedTests()
        {
            var services = new ServiceCollection();
            services.AddIntegrationDependencyInjectionJsonExample();
            var container = services.BuildServiceProvider();

            simpleTestRepository = container.GetService<ISimpleTestRepository>() ?? throw new InvalidOperationException("Could not create test repository");
            adoScopeFactory = container.GetService<IAdoScopeFactory>() ?? throw new InvalidOperationException("Could not create ado scope factory");

            SqliteTestPreparation.CreateSqliteSchema();
        }

        [TestCategory("IntegrationTestsOnCI"), TestMethod]
        public void SqliteAdoScopeBasicDistributedTest()
        {
            using (IAdoScope adoScope = adoScopeFactory.Create())
            {
                // Create databases and schemas
                simpleTestRepository.CreateDatabase();

                // Create a test entity
                var newTestEntity = new TestEntity { Name = "CreateTest", Description = "Test Description", Quantity = 1 };

                // Call our repository to add the entity
                simpleTestRepository.Add(newTestEntity);
                simpleTestRepository.AddWithDifferentContext(newTestEntity);

                // Complete data related work
                adoScope.Complete();
            }

            using (IAdoScope adoScope = adoScopeFactory.Create())
            {
                // Get the entity from the database
                TestEntity testEntity = simpleTestRepository.GetEntityByName("CreateTest");

                // Assert that the entity was retrieved
                Assert.IsNotNull(testEntity);
            }
        }

        [TestCategory("IntegrationTestsOnCI"), TestMethod]
        public void SqliteAdoScopeDistributedTest()
        {
            int recordCountBefore = GetRecordCountFirstContext();

            using (IAdoScope adoScope1 = adoScopeFactory.Create())
            {
                // Create a test entity
                var newTestEntity = new TestEntity { Name = "CreateTest", Description = "Test Description", Quantity = 1 };

                try
                {
                    // Call our repository to add the entity
                    simpleTestRepository.Add(newTestEntity);
                    simpleTestRepository.AddWithDifferentContext(newTestEntity);
                    // Sqlite won't throw an exception for divide by zero! So need another way to break it.
                    simpleTestRepository.BreakSqlite();

                    // Complete data related work
                    adoScope1.Complete();
                }
                catch
                {
                    // Do nothing. We expect this to fail.
                    adoScope1.Dispose();
                }
            }

            // If our distributed transaction has worked correctly, we shouldn't have additional records in the database.
            Assert.AreEqual(recordCountBefore, GetRecordCountFirstContext());
        }

        private int GetRecordCountFirstContext()
        {
            using IAdoScope adoScope = adoScopeFactory.Create();
            return simpleTestRepository.GetEntityCount();
        }
    }
}
