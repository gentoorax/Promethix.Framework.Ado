using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Promethix.Framework.Ado.Interfaces;
using Promethix.Framework.Ado.Tests.DependencyInjection;
using Promethix.Framework.Ado.Tests.TestSupport.DataAccess;
using Promethix.Framework.Ado.Tests.TestSupport.Entities;
using System.Data;
using System.Data.Common;

namespace Promethix.Framework.Ado.Tests.IntegrationTests
{
    [TestClass]
    public class AdoScopeTests
    {
        private readonly ITestRepository testRepository;

        private readonly IAdoScopeFactory adoScopeFactory;

        public AdoScopeTests()
        {
            var services = new ServiceCollection();
            services.AddIntegrationDependencyInjection();
            var container = services.BuildServiceProvider();

            testRepository = container.GetService<ITestRepository>() ?? throw new InvalidOperationException("Could not create test repository");
            adoScopeFactory = container.GetService<IAdoScopeFactory>() ?? throw new InvalidOperationException("Could not create ado scope factory");

            if (!File.Exists("mydatabase.db"))
            {
                File.Create("mydatabase.db").Dispose();
            }

            CreateSqliteSchema();
        }

        [TestMethod, TestCategory("IntegrationTests")]
        public void SqlLiteAdoScopeCreateTest()
        {
            using IAdoScope adoScope = adoScopeFactory.Create();

            // Create a test entity
            var newTestEntity = new TestEntity { Name = "CreateTest", Description = "Test Description", Quantity = 1 };

            // Call our repository to add the entity
            testRepository.Add(newTestEntity);

            // Complete data related work
            adoScope.Complete();

            Assert.IsNotNull(adoScope);
        }

        [TestMethod, TestCategory("IntegrationTests")]
        public void SqliteAdoScopeCreateReadTest()
        {
            using IAdoScope adoScope = adoScopeFactory.Create();
            TestEntity existingTestEntity = testRepository.GetEntityByName("Test3");
            adoScope.Complete();

            Assert.IsNotNull(existingTestEntity);
            Assert.AreEqual("Test3", existingTestEntity.Name);
            Assert.AreEqual("Test Description 3", existingTestEntity.Description);
            Assert.AreEqual(3, existingTestEntity.Quantity);    
        }

        [TestMethod, TestCategory("IntegrationTests")]
        public void SqliteAdoScopeCreateWithTransactionTest()
        {
            using IAdoScope adoScope = adoScopeFactory.CreateWithTransaction(IsolationLevel.ReadCommitted);
            testRepository.Add(new TestEntity { Name = "TransactionTest", Description = "Test Description", Quantity = 1 });
            testRepository.Add(new TestEntity { Name = "TransactionTest2", Description = "Test Description", Quantity = 1 });
            adoScope.Complete();

            Assert.IsNotNull(adoScope);
        }

        [TestCategory("IntegrationTests"), TestMethod]
        public void SqliteAdoScopeTransactionDisposeTest()
        {
            using IAdoScope adoScope = adoScopeFactory.CreateWithTransaction(IsolationLevel.ReadCommitted);
            testRepository.Add(new TestEntity { Name = "TransactionTest", Description = "Test Description", Quantity = 1 });
            testRepository.Add(new TestEntity { Name = "TransactionTest2", Description = "Test Description", Quantity = 1 });

            Assert.IsNotNull(testRepository.GetEntityByName("TransactionTest"));

            // Don't complete the transaction
            adoScope.Dispose();

            // Assert that the transaction was rolled back
            using IAdoScope adoScope2 = adoScopeFactory.Create();
            Assert.IsNull(testRepository.GetEntityByName("TransactionTest"));
        }

        #region Nested Scope Tests

        [TestMethod, TestCategory("IntegrationTests")]
        public void SqliteAdoScopeNestedAndSequentialTest()
        {
            // Testing nested scopes
            using (IAdoScope adoScope1 = adoScopeFactory.Create())
            {
                testRepository.Add(new TestEntity { Name = "NestedTest", Description = "Test Description", Quantity = 1 });

                using (IAdoScope adoScope2 = adoScopeFactory.Create())
                {
                    testRepository.Add(new TestEntity { Name = "NestedTest2", Description = "Test Description", Quantity = 1 });
                    adoScope2.Complete();
                }

                testRepository.Add(new TestEntity { Name = "NestedTest3", Description = "Test Description", Quantity = 1 });

                adoScope1.Complete();
            }

            // Testing sequential
            using (IAdoScope adoScope3 = adoScopeFactory.Create())
            {
                testRepository.Add(new TestEntity { Name = "NestedTest4", Description = "Test Description", Quantity = 1 });
                adoScope3.Complete();
            }

            // Assert all created
            using IAdoScope adoScope = adoScopeFactory.Create();
            Assert.IsNotNull(testRepository.GetEntityByName("NestedTest"));
            Assert.IsNotNull(testRepository.GetEntityByName("NestedTest2"));
            Assert.IsNotNull(testRepository.GetEntityByName("NestedTest3"));
            Assert.IsNotNull(testRepository.GetEntityByName("NestedTest4"));
            adoScope.Complete();
        }

        [TestMethod, TestCategory("IntegrationTests")]
        public void SqliteAdoScopeTransactionNestedTest()
        {
            using IAdoScope adoScope = adoScopeFactory.CreateWithTransaction(IsolationLevel.ReadCommitted);
            testRepository.Add(new TestEntity { Name = "TransactionNestedTest", Description = "Nested Transaction Test Description", Quantity = 1 });

            using IAdoScope adoScope1 = adoScopeFactory.Create();
            testRepository.Add(new TestEntity { Name = "TransactionNestedTest2", Description = "Nested Transaction Test Description", Quantity = 1 });
            adoScope1.Complete();

            adoScope.Complete();
        }

        #endregion

        #region Traditional Sqlite Initialization

        /// <summary>
        /// This method is just for SQLite initialisation and creating a test schema.
        /// It doesn't use AdoScope.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        private static void CreateSqliteSchema()
        {
            // Old school methods to create a database using SQL here.
            // We just want a table to work with.
            DbProviderFactory factory = DbProviderFactories.GetFactory("Microsoft.Data.Sqlite");

            IDbConnection connection = factory.CreateConnection() ?? throw new InvalidOperationException("Could not create database connection");
            connection.ConnectionString = "Data Source=mydatabase.db";

            connection.Open();

            using var command = new SqliteCommand("DROP TABLE TestEntity; CREATE TABLE TestEntity (Id INTEGER PRIMARY KEY, Name TEXT, Description TEXT, Quantity INTEGER)", (SqliteConnection)connection);
            command.ExecuteNonQuery();

            // Create three static test entities
            var testEntities = new List<TestEntity>
            {
                new TestEntity { Name = "Test", Description = "Test Description", Quantity = 1 },
                new TestEntity { Name = "Test2", Description = "Test Description 2", Quantity = 2 },
                new TestEntity { Name = "Test3", Description = "Test Description 3", Quantity = 3 }
            };

            foreach (var testEntity in testEntities)
            {
                using var insertCommand = new SqliteCommand("INSERT INTO TestEntity (Name, Description, Quantity) VALUES (@Name, @Description, @Quantity)", (SqliteConnection)connection);
                insertCommand.Parameters.AddWithValue("@Name", testEntity.Name);
                insertCommand.Parameters.AddWithValue("@Description", testEntity.Description);
                insertCommand.Parameters.AddWithValue("@Quantity", testEntity.Quantity);
                insertCommand.ExecuteNonQuery();
            }

            connection.Close();
            connection.Dispose();
        }

        #endregion

    }
}
