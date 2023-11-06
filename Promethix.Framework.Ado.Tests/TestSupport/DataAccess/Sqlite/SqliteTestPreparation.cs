using Microsoft.Data.Sqlite;
using Promethix.Framework.Ado.Tests.TestSupport.Entities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Tests.TestSupport.DataAccess.Sqlite
{
    internal static class SqliteTestPreparation
    {
        /// <summary>
        /// This method is just for SQLite initialisation and creating a test schema.
        /// It doesn't use AdoScope.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        internal static void CreateSqliteSchema()
        {
            // Old school methods to create a database using SQL here.
            // We just want a table to work with.
            DbProviderFactory factory = DbProviderFactories.GetFactory("Microsoft.Data.Sqlite");

            IDbConnection connection = factory.CreateConnection() ?? throw new InvalidOperationException("Could not create database connection");
            connection.ConnectionString = "Data Source=mydatabase.db";

            connection.Open();

            using var command = new SqliteCommand("DROP TABLE IF EXISTS TestEntity; CREATE TABLE TestEntity (Id INTEGER PRIMARY KEY, Name TEXT, Description TEXT, Quantity INTEGER)", (SqliteConnection)connection);
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
    }
}
