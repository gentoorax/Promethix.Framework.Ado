using Dapper;
using Microsoft.Data.SqlClient;
using Promethix.Framework.Ado.Interfaces;
using Promethix.Framework.Ado.Tests.TestSupport.Entities;
using System.Data;
using static Dapper.SqlMapper;

namespace Promethix.Framework.Ado.Tests.TestSupport.DataAccess.Sqlite
{
    public class SimpleTestRepository : ISimpleTestRepository
    {
        private readonly IAmbientAdoContextLocator ambientAdoContextLocator;

        private IDbConnection SqliteConnection1 => ambientAdoContextLocator.GetContext<SqliteContextExample1>().Connection;

        private IDbConnection SqliteConnection3 => ambientAdoContextLocator.GetContext<SqliteContextExample3>().Connection;

        public SimpleTestRepository(IAmbientAdoContextLocator ambientAdoContextLocator)
        {
            this.ambientAdoContextLocator = ambientAdoContextLocator;
        }

        public void CreateDatabase()
        {
            const string query = "DROP TABLE IF EXISTS TestEntity; CREATE TABLE TestEntity (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, Description TEXT, Quantity INTEGER)";
            SqliteConnection1.Execute(query);
            SqliteConnection3.Execute(query);
        }

        public void Add(TestEntity entity)
        {
            const string query = "INSERT INTO TestEntity (Name, Description, Quantity) VALUES (@Name, @Description, @Quantity)";
            SqliteConnection1.Execute(query, entity);
        }

        public void AddWithDifferentContext(TestEntity entity)
        {
            const string query = "INSERT INTO TestEntity (Name, Description, Quantity) VALUES (@Name, @Description, @Quantity)";
            SqliteConnection3.Execute(query, entity);
        }

        public void BreakSqlite()
        {
            const string query = "Malformed command 1 / 0";
            SqliteConnection3.Execute(query);
        }

        public TestEntity? GetEntityByName(string name)
        {
            const string query = "SELECT * FROM TestEntity WHERE Name = @Name";
            return SqliteConnection1.QuerySingleOrDefault<TestEntity>(query, new { Name = name });
        }

        public int GetEntityCount()
        {
            const string query = "SELECT COUNT(*) FROM TestEntity";
            return SqliteConnection1.ExecuteScalar<int>(query);
        }
    }
}
