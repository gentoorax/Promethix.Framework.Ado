using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Promethix.Framework.Ado.Interfaces;
using Promethix.Framework.Ado.Tests.DependencyInjection;
using Promethix.Framework.Ado.Tests.TestSupport.DataAccess.Sqlite;
using Promethix.Framework.Ado.Tests.TestSupport.Entities;
using System.Data;
using System.Data.Common;

namespace Promethix.Framework.Ado.Tests.IntegrationTests
{
    [TestClass]
    public class AdoScopeSqliteTests
    {
        private readonly ISimpleTestRepository simpleTestRepository;

        private readonly IMultiTestRepository multiTestRepository;

        private readonly IAdoScopeFactory adoScopeFactory;

        public AdoScopeSqliteTests()
        {
            var services = new ServiceCollection();
            services.AddIntegrationDependencyInjectionHybridExample();
            var container = services.BuildServiceProvider();

            simpleTestRepository = container.GetService<ISimpleTestRepository>() ?? throw new InvalidOperationException("Could not create test repository");
            multiTestRepository = container.GetService<IMultiTestRepository>() ?? throw new InvalidOperationException("Could not create test repository");
            adoScopeFactory = container.GetService<IAdoScopeFactory>() ?? throw new InvalidOperationException("Could not create ado scope factory");

            SqliteTestPreparation.CreateSqliteSchema();
        }

        [TestCategory("IntegrationTestsOnCI"), TestMethod]
        public void SqliteAdoScopeCreateTest()
        {
            using IAdoScope adoScope = adoScopeFactory.Create();

            // Create a test entity
            var newTestEntity = new TestEntity { Name = "CreateTest", Description = "Test Description", Quantity = 1 };

            // Call our repository to add the entity
            simpleTestRepository.Add(newTestEntity);

            // Complete data related work
            adoScope.Complete();

            Assert.IsNotNull(adoScope);
        }

        [TestCategory("IntegrationTestsOnCI"), TestMethod]
        public void SqliteAdoScopeCreateSurpressTest()
        {
            using IAdoScope adoScope = adoScopeFactory.Create();

            // Create a test entities
            var newTestEntity = new TestEntity { Name = "CreateTest", Description = "Test Description", Quantity = 1 };
            var newTestEntity2 = new TestEntity { Name = "CreateTest2", Description = "Test Description", Quantity = 1 };
            var entityList = new List<TestEntity> { newTestEntity, newTestEntity2 };

            // You MUST call SuppressAmbientContext() when kicking off a parallel execution flow 
            // within a AdoScope. Otherwise, this AdoScope will remain the ambient scope
            // in the parallel flows of execution, potentially leading to multiple threads
            // accessing the same AdoContext instance.
            using (adoScopeFactory.SuppressAmbientContext())
            {
                Parallel.ForEach(entityList, AddEntity);
            }

            // Note: Complete() isn't going to do anything in this instance since all the changes
            // were actually made and saved in separate AdoScopes created in separate threads.
            adoScope.Complete();
        }

        private void AddEntity(TestEntity testEntity)
        {
            using IAdoScope adoScope = adoScopeFactory.Create();
            simpleTestRepository.Add(testEntity);
            adoScope.Complete();
        }


        /// <summary>
        /// Distributed transactions are now available, but not used in this test.
        /// So this is best effort, as per DbContextScope.
        /// It's unlikely you would have multiple contexts all with different settings in the same repository
        /// like this, but this is just to demonstrate the different configuration options.
        /// </summary>
        [TestCategory("IntegrationTestsOnCI"), TestMethod]
        public void SqliteMultiContextAdoScopeCreateTest()
        {
            using (IAdoScope adoScope = adoScopeFactory.Create())
            {

                // Create the database if it doesn't exist
                multiTestRepository.CreateDatabase();

                // Create a test entity
                var newTestEntity = new TestEntity { Name = "CreateTest", Description = "Test Description", Quantity = 1 };

                // Call our repository to add the entity
                multiTestRepository.Add(newTestEntity);

                // Complete data related work
                adoScope.Complete();
            }

            using (IAdoScope adoScope = adoScopeFactory.Create())
            {
                // Assert that the entity exists in all contexts
                Assert.IsTrue(multiTestRepository.ConfirmEntityExists(new TestEntity { Name = "CreateTest", Description = "Test Description", Quantity = 1 }));
                adoScope.Complete();
            }
        }

        [TestCategory("IntegrationTestsOnCI"), TestMethod]
        public void SqliteAdoScopeCreateReadTest()
        {
            using IAdoScope adoScope = adoScopeFactory.Create();
            TestEntity? existingTestEntity = simpleTestRepository.GetEntityByName("Test3");
            adoScope.Complete();

            Assert.IsNotNull(existingTestEntity);
            Assert.AreEqual("Test3", existingTestEntity.Name);
            Assert.AreEqual("Test Description 3", existingTestEntity.Description);
            Assert.AreEqual(3, existingTestEntity.Quantity);    
        }

        /// <summary>
        /// Please note as per DbContextScope this creates a dedicated connection for an explicit transaction.
        /// So be careful, there are transaction options that can be set on the ADO context.
        /// </summary>
        [TestCategory("IntegrationTestsOnCI"), TestMethod]
        public void SqliteAdoScopeCreateWithTransactionTest()
        {
            using IAdoScope adoScope = adoScopeFactory.CreateWithTransaction(IsolationLevel.ReadCommitted);
            simpleTestRepository.Add(new TestEntity { Name = "TransactionTest", Description = "Test Description", Quantity = 1 });
            simpleTestRepository.Add(new TestEntity { Name = "TransactionTest2", Description = "Test Description", Quantity = 1 });
            adoScope.Complete();

            Assert.IsNotNull(adoScope);
        }

        [TestCategory("IntegrationTestsOnCI"), TestMethod]
        public void SqliteAdoScopeTransactionDisposeTest()
        {
            using IAdoScope adoScope = adoScopeFactory.CreateWithTransaction(IsolationLevel.ReadCommitted);
            simpleTestRepository.Add(new TestEntity { Name = "TransactionTest", Description = "Test Description", Quantity = 1 });
            simpleTestRepository.Add(new TestEntity { Name = "TransactionTest2", Description = "Test Description", Quantity = 1 });

            Assert.IsNotNull(simpleTestRepository.GetEntityByName("TransactionTest"));

            // Don't complete the transaction
            adoScope.Dispose();

            // Assert that the transaction was rolled back
            using IAdoScope adoScope2 = adoScopeFactory.Create();
            Assert.IsNull(simpleTestRepository.GetEntityByName("TransactionTest"));
        }

        [TestCategory("IntegrationTestsOnCI"), TestMethod]
        public void SqliteAdoScopeBasicDistributedTest()
        {
            using (IAdoScope adoScope = adoScopeFactory.CreateWithDistributedTransaction())
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
                TestEntity? testEntity = simpleTestRepository.GetEntityByName("CreateTest");

                // Assert that the entity was retrieved
                Assert.IsNotNull(testEntity);
            }
        }

        [TestCategory("IntegrationTestsOnCI"), TestMethod]
        public void SqliteAdoScopeDistributedTest()
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

        #region Nested Scope Tests

        [TestCategory("IntegrationTestsOnCI"), TestMethod]
        public void SqliteAdoScopeNestedAndSequentialTest()
        {
            // Testing nested scopes
            using (IAdoScope adoScope1 = adoScopeFactory.Create())
            {
                simpleTestRepository.Add(new TestEntity { Name = "NestedTest", Description = "Test Description", Quantity = 1 });

                using (IAdoScope adoScope2 = adoScopeFactory.Create())
                {
                    simpleTestRepository.Add(new TestEntity { Name = "NestedTest2", Description = "Test Description", Quantity = 1 });
                    adoScope2.Complete();
                }

                simpleTestRepository.Add(new TestEntity { Name = "NestedTest3", Description = "Test Description", Quantity = 1 });

                adoScope1.Complete();
            }

            // Testing sequential
            using (IAdoScope adoScope3 = adoScopeFactory.Create())
            {
                simpleTestRepository.Add(new TestEntity { Name = "NestedTest4", Description = "Test Description", Quantity = 1 });
                adoScope3.Complete();
            }

            // Assert all created
            using IAdoScope adoScope = adoScopeFactory.Create();
            Assert.IsNotNull(simpleTestRepository.GetEntityByName("NestedTest"));
            Assert.IsNotNull(simpleTestRepository.GetEntityByName("NestedTest2"));
            Assert.IsNotNull(simpleTestRepository.GetEntityByName("NestedTest3"));
            Assert.IsNotNull(simpleTestRepository.GetEntityByName("NestedTest4"));
            adoScope.Complete();
        }

        [TestCategory("IntegrationTestsOnCI"), TestMethod]
        public void SqliteAdoScopeTransactionNestedTest()
        {
            using IAdoScope adoScope = adoScopeFactory.CreateWithTransaction(IsolationLevel.ReadCommitted);
            simpleTestRepository.Add(new TestEntity { Name = "TransactionNestedTest", Description = "Nested Transaction Test Description", Quantity = 1 });

            using IAdoScope adoScope1 = adoScopeFactory.Create();
            simpleTestRepository.Add(new TestEntity { Name = "TransactionNestedTest2", Description = "Nested Transaction Test Description", Quantity = 1 });
            adoScope1.Complete();

            adoScope.Complete();
        }

        #endregion

        // To be re-instated once sqlite locking issue is resolved
        //[AssemblyCleanup]
        //public static void AssemblyCleanup()
        //{
        //    if (File.Exists("mydatabase.db"))
        //    {
        //        File.Delete("mydatabase.db");
        //    }

        //    if (File.Exists("mydatabase2.db"))
        //    {
        //        File.Delete("mydatabase2.db");
        //    }

        //    if (File.Exists("mydatabase3.db"))
        //    {
        //        File.Delete("mydatabase3.db");
        //    }

        //    if (File.Exists("mydatabase4.db"))
        //    {
        //        File.Delete("mydatabase4.db");
        //    }

        //    if (File.Exists("mydatabase5.db"))
        //    {
        //        File.Delete("mydatabase5.db");
        //    }
        //}
    }
}
