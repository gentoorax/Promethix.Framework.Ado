using Dapper;
using Microsoft.Data.Sqlite;
using Promethix.Framework.Ado.Interfaces;
using Promethix.Framework.Ado.Tests.TestSupport.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Tests.TestSupport.DataAccess.Sqlite
{
    public class MultiTestRepository : IMultiTestRepository
    {
        private readonly IAmbientAdoContextLocator ambientAdoContextLocator;

        public MultiTestRepository(IAmbientAdoContextLocator ambientAdoContextLocator)
        {
            this.ambientAdoContextLocator = ambientAdoContextLocator;
        }

        private IDbConnection SqliteConnection2 => ambientAdoContextLocator.GetContext<SqliteContextExample2>().Connection;

        private IDbConnection SqliteConnection3 => ambientAdoContextLocator.GetContext<SqliteContextExample3>().Connection;

        private IDbConnection SqliteConnection4 => ambientAdoContextLocator.GetContext<SqliteContextExample4>().Connection;

        private IDbConnection SqliteConnection5 => ambientAdoContextLocator.GetContext<SqliteContextExample5>().Connection;

        public void CreateDatabase()
        {
            const string query = "DROP TABLE IF EXISTS TestEntity; CREATE TABLE TestEntity (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, Description TEXT, Quantity INTEGER)";
            SqliteConnection2.Execute(query);
            SqliteConnection3.Execute(query);
            SqliteConnection4.Execute(query);
            SqliteConnection5.Execute(query);
        }

        public void Add(TestEntity entity)
        {
            const string query = "INSERT INTO TestEntity (Name, Description, Quantity) VALUES (@Name, @Description, @Quantity)";
            SqliteConnection2.Execute(query, entity);
            SqliteConnection3.Execute(query, entity);
            SqliteConnection4.Execute(query, entity);
            SqliteConnection5.Execute(query, entity);
        }

        public bool ConfirmEntityExists(TestEntity entity)
        {
            const string query = "SELECT COUNT(*) FROM TestEntity WHERE Name = @Name AND Description = @Description AND Quantity = @Quantity";
            return SqliteConnection2.ExecuteScalar<int>(query, entity) == 1
                && SqliteConnection3.ExecuteScalar<int>(query, entity) == 1
                && SqliteConnection4.ExecuteScalar<int>(query, entity) == 1
                && SqliteConnection5.ExecuteScalar<int>(query, entity) == 1;
        }
    }
}
