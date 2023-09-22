using Dapper;
using Promethix.Framework.Ado.Interfaces;
using Promethix.Framework.Ado.Tests.TestSupport.Entities;
using System.Data;

namespace Promethix.Framework.Ado.Tests.TestSupport.DataAccess
{
    public class TestRepository : ITestRepository
    {
        private readonly IAmbientAdoContextLocator ambientAdoContextLocator;

        public TestRepository(IAmbientAdoContextLocator ambientAdoContextLocator)
        {
            this.ambientAdoContextLocator = ambientAdoContextLocator;
        }

        private IDbConnection SqliteConnection => ambientAdoContextLocator.GetContext<SqliteContext>().Connection;

        public void Add(TestEntity entity)
        {
            const string query = "INSERT INTO TestEntity (Name, Description, Quantity) VALUES (@Name, @Description, @Quantity)";
            SqliteConnection.Execute(query, entity);
        }

        public TestEntity GetEntityByName(string name)
        {
            const string query = "SELECT * FROM TestEntity WHERE Name = @Name";
            return SqliteConnection.QuerySingleOrDefault<TestEntity>(query, new { Name = name });
        }
    }
}
