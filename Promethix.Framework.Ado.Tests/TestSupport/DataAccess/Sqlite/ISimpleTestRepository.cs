using Promethix.Framework.Ado.Tests.TestSupport.Entities;

namespace Promethix.Framework.Ado.Tests.TestSupport.DataAccess.Sqlite
{
    public interface ISimpleTestRepository
    {
        void Add(TestEntity entity);

        TestEntity GetEntityByName(string name);
    }
}