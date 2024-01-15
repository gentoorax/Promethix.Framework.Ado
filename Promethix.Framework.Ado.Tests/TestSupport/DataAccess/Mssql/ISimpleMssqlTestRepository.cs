using Promethix.Framework.Ado.Tests.TestSupport.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promethix.Framework.Ado.Tests.TestSupport.DataAccess.Mssql
{
    public interface ISimpleMssqlTestRepository
    {
        void Add(TestEntity entity);

        TestEntity GetEntityByName(string name);

        IEnumerable<TestEntity> GetEntities();

        void AddWithDifferentContext(TestEntity entity);

        int GetEntityCount();

        void DivideByZero();

        void DeleteAll();
    }
}
