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
    [TestClass]
    public class AdoScopeMssqlTests
    {
        private readonly ISimpleMssqlTestRepository simpleTestRepository;

        private readonly IAdoScopeFactory adoScopeFactory;

        public AdoScopeMssqlTests()
        {
            var services = new ServiceCollection();
            services.AddIntegrationDependencyInjection();
            var container = services.BuildServiceProvider();

            simpleTestRepository = container.GetService<ISimpleMssqlTestRepository>() ?? throw new InvalidOperationException("Could not create test repository");
            //multiTestRepository = container.GetService<IMultiTestRepository>() ?? throw new InvalidOperationException("Could not create test repository");
            adoScopeFactory = container.GetService<IAdoScopeFactory>() ?? throw new InvalidOperationException("Could not create ado scope factory");
        }

        [TestMethod]
        public void BasicMssqlTest()
        {
            using IAdoScope adoScope = adoScopeFactory.CreateWithDistributedTransaction();

            // Create a test entity
            var newTestEntity = new TestEntity { Name = "CreateTest", Description = "Test Description", Quantity = 1 };

            // Call our repository to add the entity
            simpleTestRepository.Add(newTestEntity);

            // Complete data related work
            adoScope.Complete();

            Assert.IsNotNull(adoScope);
        }

        [TestMethod]
        public void DistributedMssqlTest()
        {
            using IAdoScope adoScope = adoScopeFactory.CreateWithDistributedTransaction();

            // Create a test entity
            var newTestEntity = new TestEntity { Name = "CreateTest", Description = "Test Description", Quantity = 1 };

#pragma warning disable CS0168 // Variable is declared but never used
            try
            {
                // Call our repository to add the entity
                simpleTestRepository.Add(newTestEntity);
                simpleTestRepository.AddWithSecondContext(newTestEntity);
                //simpleTestRepository.DivideByZero();

                // Complete data related work
                adoScope.Complete();
            }
            catch (Exception ex)
            {
                // Do nothing. We expect this to fail.
                adoScope.Dispose();
            }
#pragma warning restore CS0168 // Variable is declared but never used

            Assert.IsTrue(true);
        }
    }
}
