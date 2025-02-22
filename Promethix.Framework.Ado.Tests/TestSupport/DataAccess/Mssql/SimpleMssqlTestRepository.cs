﻿using Dapper;
using Promethix.Framework.Ado.Interfaces;
using Promethix.Framework.Ado.Tests.TestSupport.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Promethix.Framework.Ado.Tests.TestSupport.DataAccess.Mssql
{
    public class SimpleMssqlTestRepository : ISimpleMssqlTestRepository
    {
        private readonly IAmbientAdoContextLocator ambientAdoContextLocator;

        public SimpleMssqlTestRepository(IAmbientAdoContextLocator ambientAdoContextLocator)
        {
            this.ambientAdoContextLocator = ambientAdoContextLocator;
        }

        private IDbConnection SqlConnection1 => ambientAdoContextLocator.GetContext<MssqlContextExample1>().Connection;

        private IDbTransaction SqlTransaction1 => ambientAdoContextLocator.GetContext<MssqlContextExample1>().Transaction;

        private IDbConnection SqlConnection2 => ambientAdoContextLocator.GetContext<MssqlContextExample2>().Connection;

        private IDbTransaction SqlTransaction2 => ambientAdoContextLocator.GetContext<MssqlContextExample2>().Transaction;

        private IDbConnection SqlConnection3 => ambientAdoContextLocator.GetContext<MssqlContextExample3>().Connection;

        private IDbTransaction SqlTransaction3 => ambientAdoContextLocator.GetContext<MssqlContextExample3>().Transaction;

        public void Add(TestEntity entity)
        {
            const string query = "INSERT INTO TestEntity (Name, Description, Quantity) VALUES (@Name, @Description, @Quantity)";
            SqlConnection1.Execute(query, entity, SqlTransaction1);
        }

        public void AddWithDifferentContext(TestEntity entity)
        {
            const string query = "INSERT INTO TestEntity (Name, Description, Quantity) VALUES (@Name, @Description, @Quantity)";
            SqlConnection2.Execute(query, entity, SqlTransaction2);
        }

        public TestEntity? GetEntityByName(string name)
        {
            const string query = "SELECT * FROM TestEntity WHERE Name = @Name";
            return SqlConnection1.QuerySingleOrDefault<TestEntity>(query, new { Name = name }, SqlTransaction1);
        }

        public IEnumerable<TestEntity> GetEntities()
        {
            const string query = "SELECT * FROM TestEntity";
            return SqlConnection3.Query<TestEntity>(query, transaction: SqlTransaction3);
        }

        public IEnumerable<TestEntity> GetEntitiesNullTransaction()
        {
            const string query = "SELECT * FROM TestEntity";
            return SqlConnection1.Query<TestEntity>(query, transaction: SqlTransaction1);
        }

        public int GetEntityCount()
        {
            const string query = "SELECT COUNT(*) FROM TestEntity";
            return SqlConnection1.ExecuteScalar<int>(query, SqlTransaction1);
        }

        public void DivideByZero()
        {
            const string query = "SELECT 1 / 0";
            SqlConnection2.Execute(query);
        }

        public void DeleteAll()
        {
            const string query = "DELETE FROM TestEntity";
            SqlConnection1.Execute(query, SqlTransaction1);
            SqlConnection2.Execute(query, SqlTransaction2);
        }
    }
}
