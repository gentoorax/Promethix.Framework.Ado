using Microsoft.Extensions.DependencyInjection;
using Promethix.Framework.Ado.Interfaces;
using Promethix.Framework.Ado.Tests.DependencyInjection;
using Promethix.Framework.Ado.Tests.TestSupport.DataAccess.Mssql;
using Promethix.Framework.Ado.Tests.TestSupport.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Tests.IntegrationTests
{
    /// <summary>
    /// These tests have a dependency on a local SQL Server instance.
    /// Therefore are not configured to run on CI.
    /// </summary>
    [TestClass]
    public class AdoScopeMssqlTests
    {
        private readonly ISimpleMssqlTestRepository simpleTestRepository;

        private readonly IAdoScopeFactory adoScopeFactory;

        public AdoScopeMssqlTests()
        {
            var services = new ServiceCollection();
            services.AddIntegrationDependencyInjectionHybridExample();
            var container = services.BuildServiceProvider();

            simpleTestRepository = container.GetService<ISimpleMssqlTestRepository>() ?? throw new InvalidOperationException("Could not create test repository");
            adoScopeFactory = container.GetService<IAdoScopeFactory>() ?? throw new InvalidOperationException("Could not create ado scope factory");
        }

        [TestInitialize]
        public void TestInitialize()
        {
            using IAdoScope adoScope = adoScopeFactory.Create();
            simpleTestRepository.DeleteAll();
        }

        [TestCategory("IntegrationTests"), TestMethod]
        public void MssqlAdoScopeBasicTest()
        {

            using (IAdoScope adoScope = adoScopeFactory.CreateWithDistributedTransaction())
            {
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
                Assert.IsNotNull(simpleTestRepository.GetEntityByName("CreateTest"));
            }
        }

        [TestCategory("IntegrationTests"), TestMethod]
        public void MssqlAdoScopeDistributedTest()
        {
            int recordCountBefore = GetRecordCountFirstContext();

            using (IAdoScope adoScope1 = adoScopeFactory.CreateWithDistributedTransaction())
            {
                // Create a test entity
                var newTestEntity = new TestEntity { Name = "CreateTest", Description = "Test Description", Quantity = 1 };

                try
                {
                    // Call our repository to add the entity
                    simpleTestRepository.Add(newTestEntity);
                    simpleTestRepository.AddWithDifferentContext(newTestEntity);
                    simpleTestRepository.DivideByZero();

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
