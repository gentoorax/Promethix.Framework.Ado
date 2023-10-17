using Promethix.Framework.Ado.Tests.TestSupport.Entities;

namespace Promethix.Framework.Ado.Tests.TestSupport.DataAccess.Sqlite
{
    public interface ISimpleTestRepository
    {
        void Add(TestEntity entity);

        void AddWithDifferentContext(TestEntity newTestEntity);

        void BreakSqlite();

        TestEntity GetEntityByName(string name);

        int GetEntityCount();
    }
}